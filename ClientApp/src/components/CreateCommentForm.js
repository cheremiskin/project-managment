import React from 'react'
import {Form, Input, Button} from 'antd'

const {TextArea} = Input

export const CreateCommentForm = ({onCreate}) => {    
     
    const [form] = Form.useForm()
    
    return (
        <Form
            form = {form}
        >
            <Form.Item 
                name = 'content'
                rules = {[{required: true, message: 'Field should not be empty'}]}>
                <TextArea
                    showCount={true}
                    maxLength={512}
                    rows = {4}
                    placeholder = 'Leave a comment'/>
            </Form.Item>
            
            <Button 
                type = 'primary'
                onClick = {() =>
                form.validateFields().then((values) => {
                    form.resetFields()
                    onCreate(values)
                }).catch((info) => console.log(info)) 
            }>Comment</Button>
            
        </Form>
    )
}