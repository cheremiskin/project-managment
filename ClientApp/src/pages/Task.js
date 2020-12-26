import React, {useState, useEffect} from 'react'
import HttpProvider from "../HttpProvider";
import {router} from "../router";
import {Collapse, Form, Select, Spin, Button, List, Input} from "antd";
import {UserView} from "../components/dumb/user/UserView";
import {Link} from "react-router-dom";
import moment from "moment";

import {RightOutlined} from "@ant-design/icons";

import '../assets/styles/pages/Task.css'
import {TaskComment} from "../components/dumb/task/Comment";
import {CreateCommentForm} from "../components/CreateCommentForm";

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

const token = localStorage.getItem('token')

// создатель проекта 
// название проекта 
// тайтл
// контент 
// статусы, текущий статус
// дата создания
// 

export const Task = (props) => {
    const [task, setTask] = useState({})
    const [taskLoading, setTaskLoading] = useState(true)
    
    const [users, setUsers] = useState([])
    const [usersLoading, setUsersLoading] = useState(true)
    
    const [statuses, setStatuses] = useState([])
    
    const [creator, setCreator]  = useState({})
    const [project, setProject] = useState({})
    
    const [comments, setComments] = useState([])
    
    useEffect(() => {
       loadTask(props.match.params.id, token, (result) => {
           loadProject(result.projectId, token, (proj) => {
               loadUser(proj.creatorId, token, (projectCreator) => {
                   console.log('PROJECT CREATOR',projectCreator)
                   setCreator(projectCreator)
               })
               console.log('PROJECT', proj)
               setProject(proj)
           })
           console.log('TASK', result)
           setTask(result)
           setTaskLoading(false)
       } ) 
    }, [])
    
    useEffect(() => {
        HttpProvider.auth(router.task.users(props.match.params.id), token).then((users) => {
                setUsers(users)
                setUsersLoading(false)
            })
    }, [])
    
    useEffect(() => {
        HttpProvider.get(router.task.statuses()).then((result) => {
            setStatuses(result)
        })
    }, [])
    
    useEffect(() => {
        HttpProvider.auth(router.comment.list({taskId : props.match.params.id}), token)
            .then(res => { setComments(res) })
    }, [])
    
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
    
    return (
        <>
            {taskLoading ? <Spin /> :
                <>
                    <Link tag = {Link} to={`/project/${task.projectId}`}> <RightOutlined /> <b>{project.name}</b></Link>
                    <hr/>
                    <div className = 'info-bar'>
                        <span> Opened {moment(task.creationDate).format('YYYY-MM-DD')} by <Link tag = {Link} to = {`/user/${creator.id}`}> {creator.fullName} </Link> </span>
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
                        <Button className = 'task-edit-button' type = 'dashed'>edit</Button>
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
            {comments.map(c => 
            <TaskComment
                key={c.id}
                user = {{id: 2, fullName: 'Admin'}}
                comment = {c}
            />)}
            <CreateCommentForm 
                className = 'comment-create-form'
                onCreate = {uploadComment}
            />
        </div>
            
        </>
    ) 
    
}