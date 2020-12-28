import React from 'react'
import {Checkbox, Form, Input} from 'antd'

const {TextArea} = Input

export const ProjectForm = ({initialValues, form}) => {
    
    return (
        <Form
            form = {form}
            layout = 'vertical'
            name = 'project_form'
            initialValues={initialValues}
        >
            <Form.Item
                label = 'Project Name'
                name = 'name'
                rules = {[
                    { required: true, message : 'Project must have a name' }
                ]}
            >
                <Input maxLength = {255}/>
            </Form.Item>

            <Form.Item
                label = 'Description'
                name = 'description'
            >
                <TextArea
                    rows = {5}
                    showCount={true}
                />
            </Form.Item>
            
            <Form.Item
                name = 'isPrivate'>
                <Checkbox >Make project private</Checkbox>
            </Form.Item>


        </Form>
    )
    
}