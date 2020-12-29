import React, {useEffect, useState} from 'react';
import HttpProvider from '../../HttpProvider';
import {Tabs, Spin, Avatar, Button, Modal, Form, Input, DatePicker, Checkbox} from 'antd';
import {router} from '../../router'
import moment from 'moment'

import '../../assets/styles/pages/User.css'
import {Link} from "react-router-dom";
import {ProjectForm} from "../../components/ProjectForm";
import {setCreatedProjects} from "../../store/user/actions";

const {TabPane} = Tabs;

const UpdateProfileForm = ({ visible, onUpdate, onCancel, user }) => {
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
            fullName: user.fullName,
            info: user.info,
            birthDate : moment(user.birthDate, 'YYYY/MM/DD')
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

const AddProjectModal = ({onCreate, onCancel, visible}) => {
    const [form] = Form.useForm()
    
    return (
        <Modal
            visible = {visible}
            okText = 'Create'
            onCancel = {onCancel}
            onOk = {() => {
                form.validateFields()
                    .then(values => {
                        onCreate(values)
                    })
            }}
            
        >
            <ProjectForm 
                initialValues={{
                    name: '',
                    description: ''
                }} 
                form = {form}
            /> 
        </Modal>
    )
}

export const Profile = (props) => {
    const {
            user, authenticated, setUser, token, tokenChecked
        } = props
    
    const [editModalVisible, setEditModalVisible] = useState(false)
    const [addModalVisible, setAddModalVisible] = useState(false)
    const [addProjectModalVisible, setAddProjectModalVisible] = useState(false)
    
    const [userCreatedProjects, setUserCreatedProjects] = useState([])
    
    const [profileUser, setProfileUser] = useState({})
    const [profileUserLoading, setProfileUserLoading] = useState(true)
    const [profileCreatedProjects, setProfileCreatedProjects] = useState([])
    const [createdProjectsLoading, setCreatedProjectsLoading] = useState(true)
    const [profileEnrolledProjects, setProfileEnrolledProjects] = useState([])
    const [enrolledProjectsLoading, setEnrolledProjectsLoading] = useState(true)
    
    useEffect(() => {   
        if (!tokenChecked){
            return
        }
        
        const targetUserId = props.match.params.id
        
        const myProfile = authenticated && user && targetUserId === user.id 
        
        
        if (authenticated && user) {
            HttpProvider.auth(router.user.one(targetUserId), token)
                .then((profile) => {
                    setProfileUser(profile)
                    setProfileUserLoading(false)
                })

            HttpProvider.auth(router.user.createdProjects(targetUserId), token)
                .then((projects) => {
                    setProfileCreatedProjects(projects)
                    setCreatedProjectsLoading(false)
                })
            HttpProvider.auth(router.user.enrolledProjects(targetUserId), token)
                .then((projects) => {
                    setProfileEnrolledProjects(projects)
                    setEnrolledProjectsLoading(false)
                })
            HttpProvider.auth(router.user.createdProjects(user.id), token)
                .then((projects) => {
                    setUserCreatedProjects(projects)
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

    }, [tokenChecked]) 
    
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
                HttpProvider.auth_post(router.project.addUser(id, profileUser.id), {}, token)
            }
        }
        
        setEnrolledProjectsLoading(true)
        HttpProvider.auth(router.user.enrolledProjects(profileUser.id), token)
            .then((projects) => {
                setProfileEnrolledProjects(projects)
                setEnrolledProjectsLoading(false)
            })
    }
    
    const addProject = (payload) => {
        HttpProvider.auth_post(router.project.create(), payload, token)
            .then(res => {
                if (res.id) {
                    HttpProvider.auth(router.project.one(res.id), token)
                        .then(project => {
                            setCreatedProjects(prev => prev.concat([project]))
                            setProfileCreatedProjects(prev => prev.concat([project]))
                        })
                }
            })
    }

    
    if (!tokenChecked) 
        return <Spin />
    
    const myProfile = authenticated && user && user.id === parseInt(props.match.params.id)
    
    return (
        <>
            {
                profileUserLoading ? <Spin /> :
                    <div className = 'user-container'>
                        <div className = 'user-info-wrapper'>
                            <div className='user-control'>
                                <Avatar size = {82} className = 'user-avatar' src = 'https://picsum.photos/200/200?blur' />
                                {(myProfile || user && user.isAdmin )&&
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
                                <div className = 'user-bio'>{profileUser.info ? profileUser.info : 'Empty'}</div>
                            </div>
                        </div>
                        { authenticated && !myProfile && 
                        <div>
                            <Button type = 'primary' onClick = {() => setAddModalVisible(true)}>Add to project</Button>
                            <AssignToProjectModal
                                visible = {addModalVisible}
                                onCreate = {(payload) => {
                                    assignUserToProjects(payload)
                                    setAddModalVisible(false)
                                }}
                                onCancel = {() => setAddModalVisible(false)}
                                projects = {userCreatedProjects.filter(p => !profileEnrolledProjects.some(pe => pe.id === p.id))}
                            />
                        </div>
                        }
                    </div>
            }

            <Tabs defaultActiveKey = "1">
                <TabPane tab = "Created projects" key = "1">
                    {createdProjectsLoading ? <Spin /> :
                        <ul>
                            {profileCreatedProjects.map(project => <li key = {project.id}><Link to = {`/project/${project.id}`}> {project.name} </Link></li>)}
                            {myProfile &&
                                <>
                                    <Button type = 'primary' onClick = {() => setAddProjectModalVisible(true)}>Create Project</Button>
                                    <AddProjectModal 
                                        visible = {addProjectModalVisible}
                                        onCancel = {() => setAddProjectModalVisible(false)}
                                        onCreate = {(payload) => {
                                            addProject(payload) 
                                            setAddProjectModalVisible(false)
                                        }}
                                    />
                                </>
                            }
                        </ul>
                    }
                </TabPane>

                <TabPane tab = "Enrolled projects" key = "2">
                    {enrolledProjectsLoading ? <Spin /> :
                        <ul>
                            {profileEnrolledProjects.map(project => <li key = {project.id}><Link to = {`/project/${project.id}`}> {project.name} </Link></li>)}
                        </ul>
                    }
                </TabPane>
            </Tabs>
        </>
    );
}
