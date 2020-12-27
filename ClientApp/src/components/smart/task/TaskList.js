import React, { useEffect, useState } from 'react';
import {Form, Modal, Button} from 'antd'
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

import '../../../assets/styles/components/TaskList.css'
import {CreateTaskForm} from "../../CreateTaskForm";
import moment from "moment";


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
    const [createModalVisible, setCreateModalVisible] = useState(false)
    const [usersInProject, setUsersInProject] = useState([])
    
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
    
    useEffect(()=>{
        loadTasks(props.params.projectId, token, setTasks)
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
            <div className = 'task-container'>
                {
                    tasks.map((item, index) => 
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