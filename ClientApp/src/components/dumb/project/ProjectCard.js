import React from 'react';
import { Card } from 'antd';

const ProjectCard = (props) => {
    return (
        <Card 
            title={props.name} 
            extra={<a href={`/project/${props.id}`}>More</a>}
            style={{marginBottom: 20, width: 600}}
        >
            {props.description}
        </Card>
    )
}

export default ProjectCard;