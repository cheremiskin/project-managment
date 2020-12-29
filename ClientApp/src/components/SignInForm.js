import React from 'react';
import { Form, Input, Button, DatePicker, Checkbox } from 'antd';
import {connect} from 'react-redux';
import HttpProvider from '../HttpProvider';

import {setUser, setEnrolledProjects, setToken, setCreatedProjects, setTokenChecked} from "../store/user/actions";
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
            props.setToken(`Bearer ${res.access_token}`);
            
            HttpProvider.auth('/api/users/me', 'Bearer ' + res.access_token).then (
                (res) => {
                    props.setUser(res);
                    props.setTokenChecked(true)
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
   setToken, setCreatedProjects, setEnrolledProjects, setUser, setTokenChecked
}

export default connect(() => ({}), mapDispatchToProps)(SignInForm);