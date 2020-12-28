import React from 'react';
import { Form, Input, Button, DatePicker, Checkbox } from 'antd';
import {connect} from 'react-redux';
import HttpProvider from '../HttpProvider';

import {setUser, setEnrolledProjects, setToken, setCreatedProjects} from "../store/user/actions";
import {router} from "../router";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

const SignInForm = (props) => {
  const onFinish = values => {
    HttpProvider.post('api/token/', values)
    .then (
        (res) => {
            console.log('res', res);
            props.setToken(`${res.access_token}`);
            
            HttpProvider.auth('/api/users/me', res.access_token).then (
                (res) => {
                    props.setUser(res);
                    HttpProvider.auth(router.user.createdProjects(res.id)).then(props.setCreatedProjects)
                    HttpProvider.auth(router.user.enrolledProjects(res.id)).then(props.setEnrolledProjects)
                    
                }
            )
            
        }
    )
  };

  const onFinishFailed = errorInfo => {
    console.log('Failed:', errorInfo);
  };

  return (
    <Form
      {...layout}
      name="basic"
      initialValues={{ remember: true }}
      onFinish={onFinish}
      onFinishFailed={onFinishFailed}
    >
      <Form.Item
        label="Email"
        name="Email"
        rules={[{ required: true, message: 'Please input your email!' }]}
      >
        <Input />
      </Form.Item>

     
      <Form.Item
        label="Password"
        name="Password"
        rules={[{ required: true, message: 'Please input your password!' }]}
        hasFeedback
      >
        <Input.Password />
      </Form.Item>

      <Form.Item {...tailLayout}>
        <Button type="primary" htmlType="submit">
          Submit
            </Button>
      </Form.Item>
    </Form>
  );
}

const mapDispatchToProps = {
   setToken, setCreatedProjects, setEnrolledProjects, setUser 
}

export default connect(() => ({}), mapDispatchToProps)(SignInForm);