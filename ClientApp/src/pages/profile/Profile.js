import React, {useEffect, useState} from 'react';
import HttpProvider from '../../HttpProvider';
import {Tabs, Spin, Avatar, Button, Modal, Form, Input, DatePicker, Checkbox} from 'antd';
import {router} from '../../router'
import moment from 'moment'

import '../../assets/styles/pages/User.css'

const {TabPane} = Tabs;

const UpdateProfileForm = ({ visible, onUpdate, onCancel, user }) => {
  const [formUser, setUser] = useState(user)
  const [form] = Form.useForm();
  return (
    <Modal
      visible={visible}
      title="Update profile info"
      okText="Update"
      cancelText="Cancel"
      onCancel={onCancel}
      onOk={() => {
        form
          .validateFields()
          .then((values) => {
              values.birthDate = values.birthDate.format('YYYY-MM-DD')
              console.log(values)
            form.resetFields();
            onUpdate(values);
          })
          .catch((info) => {
            console.log('Validate Failed:', info);
          });
      }}
    >
      <Form
        form={form}
        layout="vertical"
        name="update_form"
        initialValues={{
            fullName: formUser.fullName,
            info: formUser.info,
            birthDate : moment(formUser.birthDate, 'YYYY/MM/DD')
        }}
      >
        <Form.Item
          name="fullName"
          label="Full Name"
        >
          <Input />
        </Form.Item>
        <Form.Item name="info" label="Bio">
          <Input.TextArea 
              rows = {5}
              showCount={true}
              maxLength={512}
          />
        </Form.Item>
        <Form.Item name="birthDate" label = "Birth Date">
          <DatePicker />
        </Form.Item>
      </Form>
    </Modal>
  );
};

const AssignToProjectModal = ({visible, onCreate, onCancel, projects}) => {
  const [form] = Form.useForm()
  const [project, setProjects] = useState(projects)
  console.log(projects)
  return (
    <Modal 
      visible = {visible}
      onOk = {() => 
        form
          .validateFields()
          .then((values) => {
            onCreate(values)
          })
          .catch((info) => {console.log(info)})}
      onCancel = {onCancel}
      okText = 'Add'
      title = 'Add user to project'>
        <Form
            form={form}
            name='add_to_projects'>
            {
                projects.map((value) =>
                    <Form.Item key={value.id} name={value.id} valuePropName='checked' initialValue={false}>
                        <Checkbox>  {value.name} </Checkbox>
                    </Form.Item>
                )
            }
        </Form>
    </Modal>
  ) 
}

export const Profile = (props) => {
    const {
            user, authenticated, setUser, userProjects, userEnrolledProjects, token,
            setUserProjects, setUserEnrolledProjects,
            profileUser, profileCreatedProjects, profileEnrolledProjects,
            setProfileEnrolledProjects, setProfileCreatedProjects, 
            setProfileUser
        } = props
    
    const [editModalVisible, setEditModalVisible] = useState(false)
    const [addModalVisible, setAddModalVisible] = useState(false)
    const [profileUserLoading, setProfileUserLoading] = useState(true)
    const [createdProjectsLoading, setCreatedProjectsLoading] = useState(true)
    const [enrolledProjectsLoading, setEnrolledProjectsLoading] = useState(true)
    
    useEffect(() => {   
        const targetUserId = props.match.params.id
        
        const myProfile = authenticated && targetUserId === user.id 
        
        console.log('My profile: ', myProfile)
        console.log('Authenticated: ', authenticated)
        console.log('My projects', userProjects)
        console.log('My enrolled projects', userEnrolledProjects)
        
        if (authenticated) {
            HttpProvider.auth(router.user.one(targetUserId), token)
                .then((profile) => {
                    setProfileUser(profile)
                    setProfileUserLoading(false)
                })

            HttpProvider.auth(router.user.createdProjects(targetUserId), token)
                .then((projects) => {
                    if (myProfile){
                        setUserProjects(projects)
                    }
                    setProfileCreatedProjects(projects)
                    setCreatedProjectsLoading(false)
                })
            HttpProvider.auth(router.user.enrolledProjects(targetUserId), token)
                .then((projects) => {
                    if (myProfile){
                        setUserEnrolledProjects(projects)
                    }
                    setProfileEnrolledProjects(projects)
                    setCreatedProjectsLoading(false)
                })
        } else {
            HttpProvider.get(router.user.one(props.match.params.id))
                .then((profile) => {
                    setProfileUser(profile)
                    setProfileUserLoading(false)
                })
            HttpProvider.get(router.user.createdProjects(targetUserId))
                .then((projects) => {
                    setProfileCreatedProjects(projects)
                    setCreatedProjectsLoading(false)
                })
            HttpProvider.get(router.user.enrolledProjects(targetUserId))
                .then((projects) => {
                    setProfileEnrolledProjects(projects)
                    setEnrolledProjectsLoading(false)
                })
        }

    }, []) 

    const onUpdateHandle = (userId, payload) => {
        HttpProvider.auth_put(router.user.one(userId), payload, token)
            .then(() => {
                HttpProvider.auth(router.user.one(userId), token).then((responseUser) => {
                    setProfileUser(responseUser)
                    if (userId === user.id){
                        setUser(responseUser)
                    }
                })
            })
    }
    
    const assignUserToProjects = (links) => {
        for (let id in links){
            if (links[id]){
                HttpProvider.auth_post(router.project.addUser(id, user.id), {}, token)
            }
        }
        
        setEnrolledProjectsLoading(true)
        HttpProvider.auth(router.user.enrolledProjects(profileUser.id), token)
            .then((projects) => {
                setProfileEnrolledProjects(projects)
                setEnrolledProjectsLoading(false)
            })
    }


    const myProfile = authenticated && user.id === parseInt(props.match.params.id)
    
    return (
        <>
            {
                profileUserLoading ? <Spin /> :
                    <div className = 'user-container'>
                        <div className = 'user-info-wrapper'>
                            <div className='user-control'>
                                <Avatar size = {82} className = 'user-avatar' src = 'https://picsum.photos/200/200?blur' />
                                {myProfile &&
                                <>
                                    <Button 
                                        type = 'dashed'
                                        className = 'edit-profile-button' 
                                        onClick = {() => setEditModalVisible(true)}> Edit </Button>
                                    
                                    <UpdateProfileForm
                                        visible = {editModalVisible}
                                        onUpdate = { (payload) => {
                                                onUpdateHandle(profileUser.id, payload)
                                                setEditModalVisible(false) 
                                            }
                                        }
                                        onCancel = {() => setEditModalVisible(false)}
                                        user = {profileUser}
                                    />
                                </>
                                }
                            </div>
                            <div className = 'info'>
                                <div className = 'user-full-name'>{profileUser.fullName}</div>
                                <span>BIO</span>
                                <div className = 'user-bio'>{profileUser.info ? user.info : 'Empty'}</div>
                            </div>
                        </div>
                        { authenticated && !myProfile &&
                        <div>
                            <Button type = 'primary' onClick = {() => setAddModalVisible(true)}>Add to project</Button>
                            <AssignToProjectModal
                                visible = {addModalVisible}
                                onCreate = {assignUserToProjects}
                                onCancel = {() => setAddModalVisible(false)}
                                projects = {userProjects.filter(p => !profileEnrolledProjects.some(pe => pe.id === p.id))}
                            />
                        </div>
                        }
                    </div>
            }

            <Tabs defaultActiveKey = "1">
                <TabPane tab = "Created projects" key = "1">
                    {createdProjectsLoading ? <Spin /> :
                        <ul>
                            {profileCreatedProjects.map(project => <li key = {project.id}>{project.name}</li>)}
                        </ul>
                    }
                </TabPane>

                <TabPane tab = "Enrolled projects" key = "2">
                    {enrolledProjectsLoading ? <Spin /> :
                        <ul>
                            {profileEnrolledProjects.map(project => <li key = {project.id}>{project.name}</li>)}
                        </ul>
                    }
                </TabPane>
            </Tabs>
        </>
    );
}
