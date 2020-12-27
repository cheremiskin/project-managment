import React from 'react';
import { Form, Input, Button, DatePicker, Checkbox } from 'antd';
import { signIn } from '../actions/signIn';
import {connect} from 'react-redux';
import HttpProvider from '../HttpProvider';

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 16 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};

const SignInForm = (props) => {
  const onFinish = values => {
    console.log('Success:', values);
    // fetch('api/token/',{
    //   method: 'POST',
    //   headers: {
    //     'Content-Type': 'application/json;charset=utf-8'
    //   },
    //   body: JSON.stringify(values)
    // })
    HttpProvider.post('api/token/', {values})
    .then (
        (res) => {
            console.log('res', res);
            props.signIn(res.access_token);
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

export default connect(() => ({}), {signIn})(SignInForm);