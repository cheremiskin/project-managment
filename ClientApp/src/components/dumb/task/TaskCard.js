import React from 'react';
import { Card } from 'antd';
import {Typography} from "antd";

import '../../../assets/styles/components/TaskCard.css'
import {Link} from "react-router-dom";
import moment from "moment";

const {Paragraph} = Typography


const TaskCard = (props) => {
    const {task, onMore, onDelete, status} = props
    
    const styles = {}
    
    if (moment(task.expirationDate).isAfter(new moment(new Date()))){
       styles.background = 'red' 
    }
    
    return (
        <Card 
            className = 'task-card'
            title= {`${task.title}  (${status})`}
            actions = {[<Link tag = {Link} to= {`/task/${task.id}`}>More</Link>, <span onClick = {onDelete}>Delete</span>]}
            style = {{styles}}
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