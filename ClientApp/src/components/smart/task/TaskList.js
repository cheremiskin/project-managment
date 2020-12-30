import React, { useEffect, useState } from 'react';
import {List, Form, Modal, Button, Select} from 'antd'
import HttpProvider from '../../../HttpProvider';
import { router } from '../../../router';
import TaskCard from '../../dumb/task/TaskCard';

import {CreateTaskForm} from "../../CreateTaskForm";
import moment from "moment";
import {connect} from "react-redux";
import {UserList} from "../../../pages/UserList";

import '../../../assets/styles/components/TaskList.css'
import openNotification from "../../../openNotification";

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
    
    const {token, canAdd, authenticated, user, project, tokenChecked} = props
    
    const [tasks, setTasks] = useState([]);
    const [taskList, setTaskList] = useState(null)
    const [createModalVisible, setCreateModalVisible] = useState(false)
    const [usersInProject, setUsersInProject] = useState([])
    const [usersInTasks, setUsersInTasks] = useState({})
    
    const [statuses, setStatuses] = useState(null)
    
    const [chosenStatus, setChosenStatus] = useState(0)
    const [chosenSortDate, setChosenSortDate] = useState(0)
    const [chosenUser, setChosenUser] = useState(0)
    
    const resetFilters = () => {
        setChosenStatus(0) 
        setChosenSortDate(0)
    }
    
    const loadTasks = (projectId, token, callback) => {
        HttpProvider.auth(router.task.list({projectId: projectId}), token).then(res => {
            console.log(res)
            callback(res)
        })
    }

    const filterByStatus = (statusId) => {
        setChosenStatus(statusId)
        setChosenSortDate(0)
        if (statusId === 0){
            setTaskList(tasks)
            return
        }

        setTaskList(tasks.filter(t => t.statusId === statusId))
    }
    
    const sortByDate = (dateId) => {
        setChosenSortDate(dateId)
        setChosenStatus(0)
        
        if (dateId === 0){
            setTaskList(tasks)
            return
        }
        
        if (dateId === 1){
            setTaskList(tasks.sort((a, b) => moment(a.creationDate).isAfter(b.creationDate) ? -1 : 1))
        }
        if (dateId === 2){
            setTaskList(tasks.sort((a, b) => moment(a.expirationDate).isAfter(b.expirationDate) ? -1 : 1))
        }
            
    }
    
    const deleteTask = (taskId)  => {
        HttpProvider.auth_delete(router.task.one(taskId), token)
            .then(() => {
                HttpProvider.auth(router.task.list({projectId : props.params.projectId}), token)
                    .then(tasks => {
                        console.log(tasks);
                        setTasks(tasks)
                        setTaskList(tasks)
                        filterByStatus(0)
                    }).catch(error => {
                        console.log(error)
                        openNotification('Failed to fetch data')
                    }) 
                })
            .catch(error => {
                console.log(error)
                openNotification('Deletion failed')
            })
    }
    
    const createTask = (values) => {
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
            }).catch(error => {
                console.log(error)
                openNotification('Creation failed')
            })
    }
    
    // const filterByUser = (userId) => {
    //     setChosenUser(userId) 
    // }
    
    
    useEffect(()=>{
        
        if (!tokenChecked)
            return
        
        if (authenticated){
            HttpProvider.auth(router.task.list({projectId : props.params.projectId}), token)
                .then((tasks) => {
                    HttpProvider.auth(router.project.users(props.params.projectId), token).then(users => {
                        setUsersInProject(users)
                    })
                    setTasks(tasks)
                    setTaskList(tasks)
                }).catch((error) => {
                    console.log(error)
                    openNotification('Unable to load tasks')
                })
        } else {
            HttpProvider.get(router.task.list({projectId : props.params.projectId}))
                .then((tasks) => {
                    HttpProvider.get(router.project.users(props.params.projectId)).then(users => {
                        setUsersInProject(users)
                    })
                    setTasks(tasks)
                    setTaskList(tasks)
                }).catch((error) => {
                console.log(error)
                openNotification('Unable to load tasks')
            })
        }

        HttpProvider.get(router.task.statuses()).then(setStatuses)
    }, [tokenChecked, tasks]);
    
    const canDeleteTasks = authenticated && user && (user.isAdmin || user.id === project.creatorId)

    return (
        <>
            <div className = 'control-panel'>
                {canAdd &&
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
                    </>
                }

                <div className = 'filters-container'>
                    <Select
                        defaultValue = {0}
                        onChange={(payload) => {
                            sortByDate(payload)
                        }}
                        value = {chosenSortDate}
                    >
                        <Option key = {0} value = {0}>Sort by date</Option>
                        <Option key = {1} value = {1}>By Creation Date</Option>
                        <Option key = {2} value = {2}>By Expiration Date</Option>
                    </Select>
                    
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
                {taskList &&
                    taskList.map((item, index) =>
                        <TaskCard
                            task = {item}
                            key={index}
                            status = {statuses ? statuses.find(s => s.id === item.statusId).name : ''}
                            deletable = {canDeleteTasks || user && item.creatorId === user.id}
                            onDelete = {() => deleteTask(item.id)}/>
                    )
                // <List
                //     grid={{ gutter: 16, column: 3 }}
                //     itemLayout="horizontal"
                //     dataSource={
                //         taskList
                //     }
                //     renderItem={item =>
                //         <TaskCard
                //             task = {item}
                //             key={item.id}
                //             status = {statuses ? statuses.find(s => s.id === item.statusId).name : ''}
                //             deletable = {canDeleteTasks || user && item.creatorId === user.id}
                //             onDelete = {() => deleteTask(item.id)}/>
                //     }
                // />
                }
            </div> 
        </>
    )
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token,
        authenticated: state.user.token !== null && state.user.tokenChecked,
        user: state.user.user,
        tokenChecked: state.user.tokenChecked
    }
}

export default connect(mapStateToProps, {})(TaskList)

