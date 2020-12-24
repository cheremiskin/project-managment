import React, { useEffect, useState } from 'react';
import {Form, Modal, Button} from 'antd'
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

import '../../../assets/styles/components/TaskList.css'
import {CreateTaskForm} from "../../CreateTaskForm";


const CreateTaskModal = ({visible, onCancel, onCreate}) => {
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
                        onCreate(values)
                    })
                    .catch(info => {
                        console.log(info)
                    })
            }}
            >
           <CreateTaskForm form = {form}/> 
        </Modal>
    )
}


const token = localStorage.getItem('token')

export const TaskList = (props) => {

    const [tasks, setTasks] = useState([]);
    
    const [createModalVisible, setCreateModalVisible] = useState(false)
    

    const deleteTask = (taskId)  => {
        HttpProvider.auth_delete(router.task.one(taskId), token).then(
            response => console.log(response) 
        )
    }
    
    const createTask = (values) => {
        HttpProvider.auth_post(router.task.create({projectId : props.params.projectId}), values, token)
            .then(response => console.log(response))
    }
    
    useEffect(()=>{
        HttpProvider.get(router.task.list({projectId: props.params.projectId})).then(res => {
            setTasks(res.map((item, index) => 
                <TaskCard 
                    task = {item} 
                    key={index} 
                    userCount={1} 
                    onDelete = {() => deleteTask(item.id)}/>))
        })
    }, []);

    return (
        <>
            <Button 
                onClick = {() => setCreateModalVisible(true)}
            >
                Create New Task
            </Button>
            <CreateTaskModal 
                visible={createModalVisible}
                onCancel={() => setCreateModalVisible(false)}
                onCreate = {(values) => {
                    createTask(values)
                }}/>

            <div className = 'task-container'>
                {tasks}
            </div> 
        </>
    )
}

export default TaskList;