import React, {useState, useEffect} from 'react'
import HttpProvider from "../HttpProvider";
import {router} from "../router";
import {Modal, Collapse, Form, Select, Spin, Button, List, Input} from "antd";
import {UserView} from "../components/dumb/user/UserView";
import {Link} from "react-router-dom";
import {connect} from 'react-redux'
import moment from "moment";

import {RightOutlined} from "@ant-design/icons";

import '../assets/styles/pages/Task.css'
import {TaskComment} from "../components/dumb/task/Comment";
import {CreateCommentForm} from "../components/CreateCommentForm";
import {CreateTaskForm} from "../components/CreateTaskForm";
import {setUser} from "../store/user/actions";

const {Option} = Select
const {Panel} = Collapse

const loadTask = (taskId, token, callback) => {
    HttpProvider.auth(router.task.one(taskId), token).then((result) => {
        callback(result) 
    })
}

const loadUser = (userId, token, callback) => {
    HttpProvider.auth(router.user.one(userId), token).then((result) => {
        callback(result)
    })
}

const loadProject = (projectId, token, callback) => {
    HttpProvider.auth(router.project.one(projectId), token).then((result) => {
        callback(result)
    })
}

const loadComment = (commentId, token, callback) => {
    HttpProvider.auth(router.comment.one(commentId), token).then(callback)
}

const loadUsersInProject = (projectId, token, callback) => {
    HttpProvider.auth(router.project.users(projectId), token).then(callback)
}

const updateTask = (taskId, token, payload, callback) => {
    HttpProvider.auth_put(router.task.one(taskId), payload, token).then(callback)
}

const token = localStorage.getItem('token')

const EditTaskModal = ({task, onEdit, onCancel, visible, assignedUsers, allUsers}) => {
    const [form] = Form.useForm()
    
    return (
        <Modal
            title = 'Edit' 
            okText = 'Edit'
            cancelText = 'Cancel'
            visible = {visible}
            onCancel = {onCancel}
            onOk = {() => {
                form.validateFields()
                    .then((values) => {

                        values.date = values.date.format('YYYY-MM-DD')
                        values.time = values.time.format('HH:mm:ss')
                        
                        values.expirationDate = values.date + 'T' + values.time;
                        delete values.date; delete values.time;
                        
                        values.assignedUsers = values.assignedUsers.map(id => parseInt(id))
                        
                        onEdit(values)
                    })
            } }
        >
            <CreateTaskForm 
                form = {form}
                users = {allUsers}
                initialValues = {{
                    title: task.title,
                    content: task.content,
                    assignedUsers: assignedUsers.map(user => user.id.toString()),
                    date: moment(new Date(task.expirationDate)),
                    time: moment(new Date(task.expirationDate))
                }}
                
            />            
        </Modal>
    )
    
}

export const Task = (props) => {
    
    const {token, user, setUser, authenticated, tokenChecked} = props
    
    const [task, setTask] = useState({})
    const [taskLoading, setTaskLoading] = useState(true)
    
    const [users, setUsers] = useState([])
    const [usersLoading, setUsersLoading] = useState(true)
    
    const [statuses, setStatuses] = useState([])
    
    const [creator, setCreator]  = useState({})
    const [project, setProject] = useState({})
    
    const [comments, setComments] = useState([])
    const [userComments, setUserComments] = useState({})
    
    const [usersInProject, setUsersInProject] = useState([])
    const [editModalVisible, setEditModalVisible] = useState(false)
    
    const [taskCreator, setTaskCreator] = useState({})
    
    useEffect(() => {  
        if (!tokenChecked)
            return
        
        if (authenticated){
            loadTask(props.match.params.id, token, (result) => {
                loadProject(result.projectId, token, (proj) => {
                    loadUser(proj.creatorId, token, (projectCreator) => {
                        setCreator(projectCreator)
                    })
                    setProject(proj)
                })
                loadUsersInProject(result.projectId, token, setUsersInProject)
                loadUser(result.creatorId, token, setTaskCreator)
                setTask(result)
                setTaskLoading(false)
            } )


            HttpProvider.auth(router.task.users(props.match.params.id), token).then((users) => {
                setUsers(users)
                setUsersLoading(false)
            })


            HttpProvider.auth(router.comment.list({taskId : props.match.params.id}), token)
                .then(res => {
                    let userIds = new Set(res.map(c => c.userId))

                    userIds.forEach((userId) => {
                        loadUser(userId, token, (user) => {
                            setUserComments(prev => {
                                let copy = {...prev}
                                copy[userId] = user
                                return copy
                            })
                        })
                    })

                    setComments(res)
                })
        } else {
            
        }
        
        HttpProvider.get(router.task.statuses()).then((result) => {
            setStatuses(result)
        })
    }, [tokenChecked])

    const uploadComment = (comment) => {
        HttpProvider.auth_post(router.comment.list({taskId: props.match.params.id}),comment, token)
            .then( res => {
                if (res.id){
                   loadComment(res.id, token, (comment) => {
                       setComments(prevComments => prevComments.concat([comment]))
                   }) 
                }
            })
    }
    
    const onStatusChange = (status) => {
        const payload = {statusId: status}
        HttpProvider.auth_put(router.task.one(task.id), payload, token)
            .then(() => {task.statusId = status.key})
    }
    
    if (!tokenChecked)
        return <Spin />
    
    return (
        <>
            {taskLoading || usersLoading ? <Spin /> :
                <>
                    <Link tag = {Link} to={`/project/${task.projectId}`}> <RightOutlined /> <b>{project.name}</b></Link>
                    <hr/>
                    <div className = 'info-bar'>
                        <span> Opened {moment(task.creationDate).format('YYYY-MM-DD')} by <Link tag = {Link} to = {`/user/${taskCreator.id}`}> {taskCreator.fullName} </Link> </span>
                        <Select
                            defaultValue = {task.statusId}
                            onChange = {onStatusChange}
                        >
                            {statuses.map(s => <Option key = {s.id} value = {s.id}>{s.name}</Option>)} 
                        </Select>
                    </div>
                    <hr/>
                    <div className = 'task-header'>
                        <h1> {task.title}</h1>
                        <Button 
                            className = 'task-edit-button' 
                            type = 'dashed' 
                            onClick = {() => {
                                setEditModalVisible(true); 
                                console.log(editModalVisible)
                            }}
                        >
                            edit
                        </Button>
                        
                        <EditTaskModal 
                            task = {task}
                            visible = {editModalVisible}
                            assignedUsers={users}
                            allUsers={usersInProject}
                            onCancel = {() => setEditModalVisible(false)} 
                            onEdit = {(values) => {
                                setEditModalVisible(false)
                                updateTask(task.id, token, values, () => {
                                    loadTask(task.id, token, (task) => {
                                        setUsersLoading(true)
                                        HttpProvider.auth(router.task.users(props.match.params.id), token).then((users) => {
                                            setUsers(users)
                                            setUsersLoading(false)
                                        })
                                        setTask(task)
                                    })
                                })
                            } }
                        />
                    </div>
                    <div> Expires on the {moment(task.expirationDate).format('YYYY-MM-DD HH:mm')}</div>
                    <h5>{task.content}</h5> 
                </>
            }
            {usersLoading ? <Spin /> :
                <>
                    <Collapse 
                        defaultActivateKey = {['1']}
                    >
                        <Panel key={'1'} header = 'Assigned users'>
                            {users.map(user =>
                                    
                                <Link tag = {Link} to = {`/user/${user.id}`}>
                                    <UserView
                                        key = {user.id}
                                        user = {user}
                                        withFullName
                                    />
                                </Link>)}
                                
                        </Panel>
                    </Collapse>
            </>
        }
        
            <div className = 'comment-section'>
                {comments.length > 0 && Object.keys(userComments).length > 0 &&
                    comments.map(c =>
                        <TaskComment
                            key={c.id}
                            user={userComments[c.userId] ?? {id : 0, fullName: 'Dummy'}}
                            comment={c}
                        />)
                }
                <CreateCommentForm
                    className = 'comment-create-form'
                    onCreate = {uploadComment}
                />
            </div>
        </>
    ) 
}

const mapStateToProps = (state) => {
    debugger
    return {
        token: state.user.token,
        user: state.user.user, 
        authenticated: state.user.token !== null
    }
}

const mapActionsToDispatch = {
    setUser
}

export default connect(mapStateToProps, mapActionsToDispatch)(Task)