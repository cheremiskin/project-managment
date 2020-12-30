import React from 'react';
import { Form, Input, Button, DatePicker, Checkbox } from 'antd';
import HttpProvider from "../HttpProvider";
import {router} from "../router";
import openNotification from "../openNotification";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 8 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 8 },
};

const RegistrationForm = (props) => {
  
  const [form] = Form.useForm()
    
  const onFinish = values => {
    HttpProvider.post('api/users/', values)
        .then(() => { 
            form.resetFields()
          props.onRegistrationSuccess()
          openNotification('Registration went successfully', '')
        })
        .catch(error => {
          openNotification('Registration failed', 'Check data')
        })
  };

  const onFinishFailed = errorInfo => {
    console.log('Failed:', errorInfo);
  };

  return (
    <Form
      {...layout}
      name="registration"
      initialValues={{ remember: true }}
      onFinish={onFinish}
      onFinishFailed={onFinishFailed}
      form = {form}
    >
      <Form.Item
        label="Email"
        name="email"
        rules={[{ required: true, message: 'Please input your email!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Full name"
        name="fullName"
        rules={[{ required: true, message: 'Please input your full name!' },
          {type: 'regexp', pattern: new RegExp("^[A-Z][a-zA-Z]{1,}(?: [A-Z][a-zA-Z]*){0,2}$"),
          message: 'Input should be your full name'}]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Info"
        name="info"
      >
        <Input.TextArea />
      </Form.Item>

      <Form.Item
        label="BirthDate"
        name="birthDate"
      >
        <DatePicker />  
      </Form.Item>
      

      <Form.Item
        label="Password"
        name="password"
        rules={[{ required: true, message: 'Please input your password!' }]}
        hasFeedback
      >
        <Input.Password />
      </Form.Item>

      <Form.Item
        name="passwordConfirm"
        label="Confirm Password"
        dependencies={['Password']}
        hasFeedback
        rules={[
          {
            required: true,
            message: 'Please confirm your password!',
          },
          ({ getFieldValue }) => ({
            validator(rule, value) {
              if (!value || getFieldValue('Password') === value) {
                return Promise.resolve();
              }
              return Promise.reject('The two passwords that you entered do not match!');
            },
          }),
        ]}
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

export default RegistrationForm;