import React from 'react';
import { Card } from 'antd';
import {Typography} from "antd";

const {Paragraph} = Typography

const TaskCard = (props) => {
    const {task, userCount, onMore, onDelete, displayDelete} = props
    
    return (
        <Card 
            title= {task.title}
            actions = {[<a href= {`/task/${task.id}`}>More</a>, <span onClick = {onDelete}>Delete</span>]}
            // extra={<a href={`/project/${props.id}`}>More</a>}
            style={{marginBottom: 20, width: 300}}
        >
            <div className = 'task-content'>
                <Paragraph ellipsis={{ 
                    rows: 4,
                    onExpand: {onMore}
                }}>{task.content}</Paragraph>
                <span id = 'user-count-holder'>+{userCount}</span>
            </div>
        </Card>
    )
}

export default TaskCard;