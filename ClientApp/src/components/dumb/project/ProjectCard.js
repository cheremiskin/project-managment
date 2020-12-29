import React from 'react';
import { Card } from 'antd';

const ProjectCard = (props) => {
    console.log(props);
    return (
        <Card 
            title={props.name} 
            extra={<a href={`/project/${props.id}`}>More</a>}
            style={{marginBottom: 20, width: 600}}
        >
            <div className="project-card__desc">
                {props.description}    
            </div>
            <div className="project-card__date">
                {props.createdAt}    
            </div>
        </Card>
    )
}

export default ProjectCard;