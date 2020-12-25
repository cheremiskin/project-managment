import React from 'react'
import {Form, Input, TimePicker, DatePicker, Select} from "antd";
import moment from 'moment'

const {TextArea} = Input
const {Option} = Select

export const CreateTaskForm = ({form, users}) => {
    
    return (
        <Form 
            form = {form}
            layout = 'vertical'
            name = 'create_task_form'
            initialValues={{
                date : moment(new Date(), 'YYYY-MM-dd'),
                time : moment(new Date(), 'HH:mm'),
                assignedUsers: []
            }}
            >
            <Form.Item 
                label = 'Title'
                name = 'title'
                rules = {[
                    { required: true, message : 'title filed should not be empty' }
                ]}
            >
                <Input />
            </Form.Item>
            
            <Form.Item 
                label = 'Content'
                name = 'content'
                rules = {[
                    { required: true, message : 'content filed should not be empty' }
                ]}
            >
                <TextArea 
                    rows = {3} 
                    showCount={true} 
                    maxLength={512}
                /> 
            </Form.Item>
            <Form.Item label = 'Expires on'>
                <Form.Item 
                    label = 'Date'
                    name = 'date'
                >
                    <DatePicker name = 'date' />
                </Form.Item>
                <Form.Item 
                    label = 'Time'
                    name = 'time'>
                    <TimePicker name = 'time' />
                </Form.Item>
            </Form.Item>
            
            <Form.Item 
                label = 'Assign users'
                name = 'assignedUsers'
            >
                <Select
                    mode = 'multiple'
                >
                    {users.map((user) => <Option key = {user.id}>{user.fullName}</Option>)}
                </Select>
            </Form.Item>
                
        </Form>
    )
}