import {Modal, Spin, Button, Form, Collapse} from 'antd';
import {CloseOutlined} from "@ant-design/icons";
import React, { useEffect, useState } from 'react';
import TaskList from '../components/smart/task/TaskList';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import {connect} from 'react-redux'
import {ProjectForm} from "../components/ProjectForm";

import '../assets/styles/pages/ProjectDetails.css'
import {Link} from "react-router-dom";
import {UserView} from "../components/dumb/user/UserView";
import moment from "moment";

const {Panel} = Collapse

const EditProjectModal = ({onEdit, onCancel, visible, project}) => {
    
    const [form] = Form.useForm()
    
    return (
        <Modal
            visible = {visible}
            okText = 'Edit'
            onCancel = {onCancel}
            onOk = {() => {
                form.validateFields()
                    .then(values => {
                        onEdit(values)
                    })
            }}
        >
            <ProjectForm 
                form = {form}
                initialValues={{
                    name: project.name,
                    description: project.description,
                    isPrivate: project.isPrivate
                }}
            />     
        </Modal>
    )
}

export const ProjectDetail = (props) => {

    const {token, authenticated, user, tokenChecked} = props
    
    const [editModalVisible, setEditModalVisible]  = useState(false)
    const [project, setProject] = useState(null);
    const [creator, setCreator] = useState(null)
    const [users, setUsers] = useState([])
    
    const [projectIsAccessible, setProjectIsAccessible] = useState(true)

    useEffect(() => {
        if (!tokenChecked)
            return 
        
        if (authenticated){
            HttpProvider.auth(router.project.one(props.match.params.id), token)
                .then(res => {
                    setProject(res);
                    HttpProvider.auth(router.user.one(res.creatorId), token)
                        .then(setCreator)
                    HttpProvider.auth(router.project.users(props.match.params.id), token)
                        .then(res => {
                        setUsers(res)
                    })
            }).catch((error) => {
                debugger
                console.log(error)
               setProjectIsAccessible(false) 
            });
            
        }
        else {
            HttpProvider.get(router.project.one(props.match.params.id)).then(res => {
                    setProject(res)
                    HttpProvider.get(router.user.one(res.creatorId))
                        .then(setCreator)
                    HttpProvider.get(router.project.users(props.match.params.id))
                        .then((user) => {
                        setUsers(user)
                    })
            }).catch((error) => {
                debugger
                console.log(error)
                setProjectIsAccessible(false)
            })
        }
        
        
    }, [tokenChecked]);
    
    if (!projectIsAccessible)
        return <h1>Oops, something went wrong</h1>
    
    
    const editProject = (payload) => {
        HttpProvider.auth_put(router.project.one(props.match.params.id), payload, token)
            .then(() => {
                HttpProvider.auth(router.project.one(props.match.params.id), token)
                    .then(setProject)
            })
    }
    
    const removeUserFromProject = (userId) => {
        HttpProvider.auth_delete(router.project.addUser(props.match.params.id, userId), token)
            .then((res) => {
                HttpProvider.auth(router.project.users(props.match.params.id), token).then(us => {
                    setUsers(us) 
                })
            })
    }

    
    if (!project || !tokenChecked || !users) 
        return <Spin />
    
    const canEdit = authenticated && user && (user.isAdmin || user.id === project.creatorId)
    
    debugger
    
    return (
        <>
            {project.isPrivate && <span className = 'project-private'>Private</span>}
            <div className = 'project-header'>
                <h1>{project.name}</h1>
                {canEdit &&
                <>
                    <Button type='dashed' onClick={() => setEditModalVisible(true)}>Edit</Button>
                    <EditProjectModal
                        visible={editModalVisible}
                        project={project}
                        onCancel={() => setEditModalVisible(false)}
                        onEdit = {(values) => {
                            editProject(values)
                            setEditModalVisible(false)
                        }}
                    />
                </>
                }
            </div>
            <div> Created by {creator ? <Link to = {`/user/${creator.id}`}>{creator.fullName}</Link> : '' } at {project ? moment(project.creationDate).format('YYYY-MM-DD HH:mm') : ''}</div>
            <p>{project.description}</p>
            
            {!users ? <Spin /> :
                <div className = 'user-list'>
                    <Collapse
                        defaultActivateKey = {['1']}
                    >
                        <Panel key={'1'} header = 'Assigned users'>
                            {users.map(u =>
                                <div className = 'user-row'>
                                    {canEdit && user.id !== u.id &&<CloseOutlined onClick = {() => removeUserFromProject(u.id)} />}
                                    <Link key = {u.id} tag = {Link} to = {`/user/${u.id}`}>
                                        <UserView
                                            user = {u}
                                            withFullName
                                        />
                                    </Link>  
                                </div>
                                )}

                        </Panel>
                    </Collapse>
                </div>
            }
            <TaskList
                id={project.id}
                project = {project}
                canAdd = {authenticated}
                params={{
                    projectId: project.id
                }}
            />
        </>
    );
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token,
        user: state.user.user,
        authenticated: state.user.token !== null && state.user.tokenChecked,
        tokenChecked: state.user.tokenChecked
    }
}

export default connect(mapStateToProps)(ProjectDetail)
