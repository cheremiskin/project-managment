import React, {useEffect, useState, useCallback} from 'react';
import HttpProvider from '../HttpProvider';
import {Tabs, Spin, Avatar, Button, Modal, Form, Input, DatePicker, Checkbox} from 'antd';
import {router} from '../router'
import moment from 'moment'

import '../assets/styles/pages/User.css'

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

export const User = (props) => {
    const [createdProjects, setCreatedProjects] = useState([])
    const [createdProjectsLoading, setCreatedProjectsLoading] = useState(true)
    
    const [enrolledProjects, setEnrolledProjects] = useState([])
    const [enrolledProjectsLoading, setEnrolledProjectsLoading] = useState(true)

    const [myProjects, setMyProjects] = useState([])
    const [myProjectsLoading, setMyProjectsLoading] = useState(true)

    const [user, setUser] = useState({})
    const [userMe, setUserMe] = useState({})
    
    const [userLoading, setUserLoading] = useState(true)
    const [userMeLoading, setUserMeLoading] = useState(true)
    
    const [editModalVisible, setEditModalVisible] = useState(false)
    const [addModalVisible, setAddModalVisible] = useState(false)

    const token = localStorage.getItem('token');

    useEffect(() => { 
        HttpProvider.auth(router.user.createdProjects(props.match.params.id), token).then((response) => {
            setCreatedProjects(response)
            setCreatedProjectsLoading(false)
        })
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.enrolledProjects(props.match.params.id), token).then((response) => {
            setEnrolledProjects(response)
            setEnrolledProjectsLoading(false)
        })
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.one(props.match.params.id), token).then((response) => {
            setUser(response)
            setUserLoading(false)
        }) 
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.me(props.match.params.id), token).then((response) => {
            setUserMe(response)
            setUserMeLoading(false)
        }) 
    }, [])

    useEffect(() => {
      HttpProvider.auth(router.user.myProjects(), token).then((response) => {
        setMyProjects(response)
        setMyProjectsLoading(false)
      })
    }, [])


    const onUpdateHandle = (payload) => {
        setUserLoading(true)
        
        payload.birthDate = payload.birthDate.format('YYYY-MM-DD');
        
        HttpProvider.auth_put(router.user.one(userMe.id), payload, token).then( response => {
                HttpProvider.auth(router.user.me(), token).then(entity => {
                    console.log('NEW ME', entity)
                    setUser(entity)
                    setUserLoading(false)
                })
        })
        
        setEditModalVisible(false)
    }

    const assignUserToProjects = (links) =>{ 
      for (let id in links){
        if (links[id]){
            HttpProvider.auth_post(router.project.addUser(id, user.id), {}, token)
        }
      }
    }
   
    const excludeProjects = (projectListTarget, projectList) => {
        return projectListTarget.filter(project => !projectList.some(p => project.id === p.id))  
    }
    
    return (
            <>
            {
            userLoading || userMeLoading ? <Spin /> :
            <div className = 'user-container'>
                <div className = 'user-info-wrapper'>
                    <div className='user-control'>
                        <Avatar size = {82} className = 'user-avatar' src = 'https://picsum.photos/200/200?blur'>{user.fullName[0]}</Avatar> 
                        {userMe.id === user.id && 
                            <> 
                                <Button className = 'edit-profile-button' onClick = {() => setEditModalVisible(true)}> Edit </Button>
                                <UpdateProfileForm 
                                    visible = {editModalVisible}
                                    onUpdate = {onUpdateHandle}
                                    onCancel = {() => setEditModalVisible(false)}
                                    user = {user}
                                    />
                            </> 
                        }
                    </div>
                    <div className = 'info'>
                        <div className = 'user-full-name'>{user.fullName}</div>
                        <span>BIO</span>
                        <div className = 'user-bio'>{user.info ? user.info : 'Empty'}</div>
                    </div>
                </div>
                { !userLoading && !userMeLoading && user.id !== userMe.id && !myProjectsLoading &&
                <div>
                  <Button type = 'primary' onClick = {() => setAddModalVisible(true)}>Add to project</Button>
                  <AssignToProjectModal
                    visible = {addModalVisible}
                    onCreate = {assignUserToProjects}
                    onCancel = {() => setAddModalVisible(false)}
                    projects = {excludeProjects(myProjects, enrolledProjects)}
                   />
                </div>
                }
            </div>
            }

            <Tabs defaultActiveKey = "1">
                <TabPane tab = "Created projects" key = "1">
                    {createdProjectsLoading ? <Spin /> : 
                        <ul>
                            {createdProjects.map(project => <li key = {project.id}>{project.name}</li>)} 
                        </ul>
                    }
                </TabPane>   

                <TabPane tab = "Enrolled projects" key = "2">
                    {enrolledProjectsLoading ? <Spin /> : 
                        <ul>
                            {enrolledProjects.map(project => <li key = {project.id}>{project.name}</li>)}
                        </ul>
                    }
                </TabPane>
            </Tabs>
            </>
        ); 
}