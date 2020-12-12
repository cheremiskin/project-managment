import React from 'react';
import { Card } from 'antd';

const TaskCard = (props) => {
    return (
        <Card 
            title="T"
            // extra={<a href={`/project/${props.id}`}>More</a>}
            style={{marginBottom: 20, width: 600}}
        >
        Task
        </Card>
    )
}

export default TaskCard;