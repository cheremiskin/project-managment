import React from 'react';
import { Form, Input, Button, DatePicker, Checkbox } from 'antd';

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

const RegistrationForm = (props) => {
  const onFinish = values => {
    console.log('Success:', values);
    fetch('api/users/',{
      method: 'POST',
      headers: {
        'Content-Type': 'application/json;charset=utf-8'
      },
      body: JSON.stringify(values)
    });
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
    >
      <Form.Item
        label="Email"
        name="Email"
        rules={[{ required: true, message: 'Please input your email!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Full name"
        name="FullName"
        rules={[{ required: true, message: 'Please input your full name!' }]}
      >
        <Input />
      </Form.Item>

      <Form.Item
        label="Info"
        name="Info"
        // rules={[{ required: true, message: 'Please input your full name!' }]}
      >
        <Input.TextArea />
      </Form.Item>

      <Form.Item
        label="BirthDate"
        name="BirthDate"
        // rules={[{ required: true, message: 'Please input your full name!' }]}
      >
        <DatePicker />  
      </Form.Item>
      

      <Form.Item
        label="Password"
        name="Password"
        rules={[{ required: true, message: 'Please input your password!' }]}
        hasFeedback
      >
        <Input.Password />
      </Form.Item>

      <Form.Item
        name="PasswordConfirm"
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