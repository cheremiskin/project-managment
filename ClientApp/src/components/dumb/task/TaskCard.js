import React from 'react';
import { Card } from 'antd';
import {Typography} from "antd";

import '../../../assets/styles/components/TaskCard.css'
import {Link} from "react-router-dom";
import moment from "moment";

const {Paragraph} = Typography


const TaskCard = (props) => {
    const {task, onMore, onDelete, status, deletable} = props
    
    let expired = false
    
    if (moment(task.expirationDate).isBefore(new moment(new Date()))){
        expired = true
    }
    
    const actions = [<Link tag = {Link} to= {`/task/${task.id}`}>More</Link>]
    if (deletable)
        actions.push(<span onClick = {onDelete}>Delete</span>)
    
    return (
        <Card 
            className = {'task-card' + (expired ? ' task-red-bordered' : '')}
            title= {`${task.title}  (${status})`}
            actions = {actions}
        >
            <div className = 'task-content'>
                <Paragraph ellipsis={{ 
                    rows: 1,
                    onExpand: {onMore}
                }}>{task.content}</Paragraph>
                <span className = 'expiration-time'>Expires at {moment(task.expirationDate).format('YYYY-MM-DD HH:mm')}</span>
            </div>
        </Card>
    )
}

export default TaskCard;