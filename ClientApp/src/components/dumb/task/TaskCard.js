import React from 'react';
import { Card } from 'antd';
import {Typography} from "antd";

import '../../../assets/styles/components/TaskCard.css'

const {Paragraph} = Typography

const TaskCard = (props) => {
    const {task, onMore, onDelete} = props
    
    return (
        <Card 
            className = 'task-card'
            title= {task.title}
            actions = {[<a href= {`/task/${task.id}`}>More</a>, <span onClick = {onDelete}>Delete</span>]}
        >
            <div className = 'task-content'>
                <Paragraph ellipsis={{ 
                    rows: 4,
                    onExpand: {onMore}
                }}>{task.content}</Paragraph>
            </div>
        </Card>
    )
}

export default TaskCard;