import React, { useEffect, useState } from 'react';
import {Form, Modal, Button, Select} from 'antd'
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

import {CreateTaskForm} from "../../CreateTaskForm";
import moment from "moment";
import {connect} from "react-redux";
import {UserList} from "../../../pages/UserList";

import '../../../assets/styles/components/TaskList.css'

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


export const TaskList = (props) => {
    
    const {token} = props
    
    const [tasks, setTasks] = useState([]);
    const [taskList, setTaskList] = useState(null)
    const [createModalVisible, setCreateModalVisible] = useState(false)
    const [usersInProject, setUsersInProject] = useState([])
    const [usersInTasks, setUsersInTasks] = useState({})
    
    const [statuses, setStatuses] = useState(null)
    
    const [chosenStatus, setChosenStatus] = useState(0)
    const [chosenUser, setChosenUser] = useState(0)
    
    const resetFilters = () => {
        setChosenStatus(0) 
        setChosenUser(0)
    }
    
    const loadTasks = (projectId, token, callback) => {
        HttpProvider.auth(router.task.list({projectId: projectId}), token).then(res => {
            console.log(res)
            callback(res)
        })
    }
    
    const deleteTask = (taskId)  => {
        HttpProvider.auth_delete(router.task.one(taskId), token)
            .then(() => loadTasks(props.params.projectId, token, (tasks) => {
                setTasks(tasks)
                filterByStatus(chosenStatus)
            }))
    }
    
    const createTask = (values) => {
        console.log(values)
        HttpProvider.auth_post(router.task.create({projectId : props.params.projectId}), values, token)
            .then((res) => {
                if (res.id)
                    HttpProvider.auth(router.task.one(res.id), token)
                        .then(task => {
                            resetFilters()
                            setTasks(prev => {
                                setTaskList(prev.concat([task]))
                                return prev.concat([task])
                            })
                        })
            })
    }
    
    // const filterByUser = (userId) => {
    //     setChosenUser(userId) 
    // }
    
    const filterByStatus = (statusId) => {
        setChosenStatus(statusId)
        if (statusId === 0){
            setTaskList(tasks)
            return
        }
        
        setTaskList(tasks.filter(t => t.statusId === statusId))
    }
    
    useEffect(()=>{
        loadTasks(props.params.projectId, token, (tasks) => {
            setTasks(tasks)
            setTaskList(tasks.sort((t1, t2) => moment(t1.expirationDate).isAfter(moment(t2))))
        })
        
        HttpProvider.get(router.task.statuses()).then(setStatuses)
    }, []);
    
    useEffect(() => {
        HttpProvider.auth(router.project.users(props.params.projectId), token).then(users => {
            setUsersInProject(users)
        })
    }, [])

    return (
        <>
            <div className = 'control-panel'>
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

                <div className = 'filters-container'>
                    {/*<Select*/}
                    {/*    defaultValue={0}*/}
                    {/*    value = {chosenUser}*/}
                    {/*    onChange={value => setChosenUser(value)}*/}
                    {/*>*/}
                    {/*    <Option key = {0} value = {0}>Member</Option>*/}
                    {/*    {usersInProject.map(user =>*/}
                    {/*        <Option key = {user.id} value = {user.id}>{user.fullName}</Option>)}*/}
                    {/*</Select>*/}
                    <Select
                        defaultValue = {0}
                        onChange={(payload) => {
                            filterByStatus(payload)
                        }}
                        value = {chosenStatus}
                    >
                        <Option key = {0} value = {0}>Status</Option>
                        {statuses && statuses.map(status => <Option key = {status.id} value = {status.id}>{status.name}</Option>)}
                    </Select>
                </div>
            </div>
            <div className = 'task-container'>
                { taskList &&
                    taskList.map((item, index) => 
                    <TaskCard
                        task = {item}
                        key={index}
                        status = {statuses ? statuses.find(s => s.id === item.statusId).name : ''}
                        onDelete = {() => deleteTask(item.id)}/>
                )}
            </div> 
        </>
    )
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token
    }
}

export default connect(mapStateToProps, {})(TaskList)

