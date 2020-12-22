import React, {useEffect, useState, useCallback} from 'react';
import HttpProvider from '../HttpProvider';
import {Tabs, Spin, Avatar, Button, Modal, Form, Input, DatePicker, Checkbox} from 'antd';
import {router} from '../router'
import moment from 'moment'

import '../assets/styles/pages/User.css'

const {TabPane} = Tabs;

const UpdateProfileForm = ({ visible, onCreate, onCancel, user }) => {
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
            onCreate(values);
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
          <Input.TextArea />
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
  const [project, setProjets] = useState(projects)
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
          form = {form}
          name = 'add_to_projects'>
            { 
              projects.map((value) => 
                <Form.Item key = {value.id} name = {value.id} valuePropName = 'checked' initialValue = {false}>
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

    const token = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWFydGluQG1haWwucnUiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJST0xFX1VTRVIiLCJVc2VySWQiOiIyIiwibmJmIjoxNjA3NzY1Mzk5LCJleHAiOjE2NjE3NjUzOTksImlzcyI6IlBtQ29ycCIsImF1ZCI6IlBtIn0.yU4oJtAbwchmWlPGHsKgaayDZ2DpBSscyskAqBGcvB4'

    useEffect(() => { 
        HttpProvider.auth(router.user.createdProjects(props.match.params.id), token).then((response) => {
            setCreatedProjects(response.map((u) => <li key = {u.id}>{u.name}</li>))
            // setCreatedProjects(response)
            setCreatedProjectsLoading(false)
        })
    }, [])

    useEffect(() => {
        HttpProvider.auth(router.user.enrolledProjects(props.match.params.id), token).then((response) => {
            setEnrolledProjects(response.map((u) => <li key = {u.id}>{u.name}</li>))
            // setEnrolledProjects(response)
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


    const onSubmitHandle = (payload) => {
        HttpProvider.auth_put(router.user.one(userMe.id), JSON.stringify(payload), token)
    }

    const assignUserToProjects = (links) =>{ 
      for (var id in links){
        if (links[id]){
          HttpProvider.auth_post(router.project.addUser(id, user.id))
        }
      }
    }
    
    return (
            <>
            {
            userLoading || userMeLoading ? <Spin /> :
            <div className = 'user-container'>
                <div className = 'user-info-wrapper'>
                    <div className='user-control'>
                        <Avatar size = {82} className = 'user-avatar' src = 'https://picsum.photos/200/200?blur'>{user.fullName[0]}</Avatar> 
                        {userMe.id == user.id && 
                            <> 
                                <Button className = 'edit-profile-button' onClick = {() => setEditModalVisible(true)}> Edit </Button>
                                <UpdateProfileForm 
                                    visible = {editModalVisible}
                                    onCreate = {onSubmitHandle}
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
                    projects = {myProjects}
                   />
                </div>
                }
            </div>
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