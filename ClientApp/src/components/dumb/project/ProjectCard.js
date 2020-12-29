import React from 'react';
import { Card } from 'antd';
import moment from "moment";

import '../../../assets/styles/components/ProjectCard.css'

const ProjectCard = (props) => {
    debugger
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
            <div className = 'project-card-bottom'>
                <div className="project-card__date">
                    Created at {moment(props.createdAt).format('YYYY-MM-DD HH:mm')}
                </div>
                {props.isPrivate &&
                <div className="project-card__private">
                    Private
                </div>
                }
            </div>
        </Card>
    )
}

export default ProjectCard;