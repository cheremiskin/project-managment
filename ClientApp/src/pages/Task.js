import React, {useState, useEffect} from 'react'
import HttpProvider from "../HttpProvider";
import {router} from "../router";
import {Collapse, Form, Select, Spin, Button} from "antd";
import {UserView} from "../components/dumb/user/UserView";
import {Link} from "react-router-dom";
import moment from "moment";
import {EditOutlined} from "@ant-design/icons"; 

import '../assets/styles/pages/Task.css'

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
            console.log('STATUSES', result)
            setStatuses(result)
        })
    }, [])
    
    const onStatusChange = (status) => {
        const payload = {statusId: status}
        HttpProvider.auth_put(router.task.one(task.id), payload, token)
            .then(() => {task.statusId = status.key})
    }
    
    return (
        <>
            {taskLoading ? <Spin /> :
                <>
                    <Link tag = {Link} to={`/project/${task.projectId}`}>Back to <b>{project.name}</b></Link>
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
                    <div className = 'task-header'>
                        <h1> {task.title}</h1>
                        <Button className = 'task-edit-button' type = 'dashed'>edit</Button>
                    </div>
                    <div> Expires on the {moment(task.expirationDate).format('YYYY-MM-DD HH:mm')}</div>
                    <h4> {task.content}</h4>
                </>
            }
            {usersLoading ? <Spin /> :
                <>
                    <Collapse defaultActivateKey = {['1']} >
                        <Panel key={'1'} header = 'Assigned users'>
                            {users.map(user =>
                                <UserView
                                    key = {user.id}
                                    user = {user}
                                    withFullName
                                />)}
                        </Panel>
                        
                    </Collapse>
            </>
        }
            
        </>
    ) 
    
}