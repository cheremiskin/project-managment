import React, {useEffect, useState} from 'react';
import HttpProvider from '../HttpProvider';
import {Tabs, Spin, Avatar, Button, Modal, Form, Input, DatePicker} from 'antd';
import {router} from '../router'

import '../assets/styles/pages/User.css'
import { resolveProjectReferencePath } from 'typescript';
import TextArea from 'antd/lib/input/TextArea';

const {TabPane} = Tabs;

export const User = (props) => {
    const [createdProjects, setCreatedProjects] = useState([])
    const [createdProjectsLoading, setCreatedProjectsLoading] = useState(true)
    const [enrolledProjects, setEnrolledProjects] = useState([])
    const [enrolledProjectsLoading, setEnrolledProjectsLoading] = useState(true)
    const [user, setUser] = useState({})
    const [userMe, setUserMe] = useState({})
    const [userLoading, setUserLoading] = useState(true)
    const [userMeLoading, setUserMeLoading] = useState(true)
    
    const [editModalVisible, setEditModalVisible] = useState(false)

    const token = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFydGluQG1haWwucnUiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJST0xFX1VTRVIiLCJVc2VySWQiOiIyIiwibmJmIjoxNjA3NzY1Mzk5LCJleHAiOjE2NjE3NjUzOTksImlzcyI6IlBtQ29ycCIsImF1ZCI6IlBtIn0.yU4oJtAbwchmWlPGHsKgaayDZ2DpBSscyskAqBGcvB4'

    useEffect(() => {
        HttpProvider.auth(router.user.createdProjects(props.match.params.id), token).then((response) => {
            setCreatedProjects(response.map((u) => <li key = {u.id}>{u.name}</li>))
            setCreatedProjectsLoading(false)
        })
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.enrolledProjects(props.match.params.id), token).then((response) => {
            setEnrolledProjects(response.map((u) => <li key = {u.id}>{u.name}</li>))
            setEnrolledProjectsLoading(false)
        })
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.one(props.match.params.id), token).then((response) => {
            setUser(response)
            setUserLoading(false)
            console.log(response)
        }) 
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.me(props.match.params.id), token).then((response) => {
            setUserMe(response)
            setUserMeLoading(false)
            console.log(response)
        }) 
    }, [])

    const showEditModal = () => {
        setEditModalVisible(true);
    }

    const handleEditOk = () => {

    }

    const handleEditCancel = () => {
        setEditModalVisible(false)
    }

    const onEditSuccess = (values) => {
        console.log(values)
        setEditModalVisible(false) 
    }


    const formItemLayout = {
          labelCol: { span: 4 },
          wrapperCol: { span: 16 },
        }
    
    return (
            <>
            {
            userLoading || userMeLoading ? <Spin /> :
            <div className = 'user-container'>
                <div className = 'user-info-wrapper'>
                    <div className='user-control'>
                        <Avatar size = {82} className = 'user-avatar'>{user.fullName[0]}</Avatar> 
                        {userMe.id == user.id && <Button className = 'edit-profile-button' onClick = {showEditModal}> Edit </Button>}
                    </div>
                    <div className = 'info'>
                        <div className = 'user-full-name'>{user.fullName}</div>
                        <span>BIO</span>
                        <div className = 'user-bio'>{user.info ? user.info : 'Empty'}</div>
                    </div>
                </div>
            </div>
            }

            { !userLoading && 
            <Modal title = 'Edit profile' visible = {editModalVisible} onCancel = {handleEditCancel} onOk = {handleEditOk}>
                <Form name = 'edit' {...formItemLayout} onFinish = {onEditSuccess}>
                    <Form.Item label = 'Full name' initialValue = {user.fullName}> <Input/></Form.Item>
                    <Form.Item label = 'Bio'> <TextArea showCount maxLength = {512} value = {user.info}/></Form.Item>
                    <Form.Item label = 'Birth Date'> <DatePicker value = {user.DatePicker}/></Form.Item>
                </Form>
            </Modal>
            }


            <Tabs defaultActiveKey = "1">
                <TabPane tab = "Created projects" key = "1">
                    {createdProjectsLoading ? <Spin /> : <ul>{createdProjects} </ul>}
                </TabPane>   

                <TabPane tab = "Enrolled projects" key = "2">
                    {enrolledProjectsLoading ? <Spin /> : <ul>{enrolledProjects}</ul>}
                </TabPane>
            </Tabs>
            </>
        ); 
}