import React, { useEffect, useState } from 'react';
import {Form, Modal, Button, Select} from 'antd'
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

import '../../../assets/styles/components/TaskList.css'
import {CreateTaskForm} from "../../CreateTaskForm";
import moment from "moment";

const {Option} = Select


const CreateTaskModal = ({visible, onCancel, onCreate, assignableUsers}) => {
    const [form] = Form.useForm();
    return (
        <Modal
            title = 'Create Task'
            okText = 'Create'
            cancelText = 'Cancel'
            visible = {visible}
            onCancel = {onCancel}
            onOk = {() => {
                form.validateFields()
                    .then(values => {
                        console.log(values)
                        form.resetFields();
                        values.date = values.date.format('YYYY-MM-DD')
                        values.time = values.time.format('HH:mm:ss')
                        
                        values.expirationDate = values.date + 'T' + values.time;
                        delete values.date; delete values.time;
                        
                        values.assignedUsers =  values.assignedUsers.map(id => parseInt(id))
                        
                        onCreate(values)
                    })
                    .catch(info => {
                        console.log(info)
                    })
            }}
            >
           <CreateTaskForm 
               form = {form} 
               users = {assignableUsers}
               initialValues={{
                   date : moment(new Date(), 'YYYY-MM-dd'),
                   time : moment(new Date(), 'HH:mm'),
                   assignedUsers: []
               }}
           /> 
        </Modal>
    )
}

const token = localStorage.getItem('token')

export const TaskList = (props) => {
    
    const [tasks, setTasks] = useState([]);
    const [taskList, setTaskList] = useState([])
    const [createModalVisible, setCreateModalVisible] = useState(false)
    const [usersInProject, setUsersInProject] = useState([])
    const [usersInTasks, setUsersInTasks] = useState({})
    
    const loadTasks = (projectId, token, callback) => {
        HttpProvider.auth(router.task.list({projectId: projectId}), token).then(res => {
            console.log(res)
            callback(res)
        })
    }
    
    const deleteTask = (taskId)  => {
        HttpProvider.auth_delete(router.task.one(taskId), token)
            .then(() => loadTasks(props.params.projectId, token, setTasks))
    }
    
    const createTask = (values) => {
        console.log(values)
        HttpProvider.auth_post(router.task.create({projectId : props.params.projectId}), values, token)
            .then(() => loadTasks(props.params.projectId, token, setTasks))
    }
    
    const filterByUser = (userId) => {
        
    }
    
    useEffect(()=>{
        loadTasks(props.params.projectId, token, (tasks) => {
            setTasks(tasks)
            setTaskList(tasks)
        })
    }, []);
    
    useEffect(() => {
        
        HttpProvider.auth(router.project.users(props.params.projectId), token).then(users => {
            setUsersInProject(users)
        })
    }, [])

    return (
        <>
            <Button id = 'create-button'
                onClick = {() => setCreateModalVisible(true)}
            >
                Create New Task
            </Button>
            <CreateTaskModal 
                visible={createModalVisible}
                onCancel={() => setCreateModalVisible(false)}
                onCreate = {(values) => {
                    createTask(values)
                    setCreateModalVisible(false)
                }}
                assignableUsers={usersInProject}
            />
            
            <Select
               defaultValue={0}
            >
                <Option key = {0} value = {0}>None</Option>
                {usersInProject.map(user => 
                    <Option key = {user.id} value = {user.id}>{user.fullName}</Option>)} 
            </Select>
            <div className = 'task-container'>
                {
                    taskList.map((item, index) => 
                    <TaskCard
                        task = {item}
                        key={index}
                        onDelete = {() => deleteTask(item.id)}/>
                )}
            </div> 
        </>
    )
}

export default TaskList;