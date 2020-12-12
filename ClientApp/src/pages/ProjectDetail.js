import { Spin } from 'antd';
import React, { useEffect, useState } from 'react';
import TaskList from '../components/smart/task/TaskList';
import HttpProvider from '../HttpProvider';
import { router } from '../router';

export const ProjectDetail = (props) => {

    const [project, setProject] = useState(null);

    useEffect(() => {
        HttpProvider.get(router.project.one(props.match.params.id)).then(res => {
            setProject(res);
        });
    }, []);

    if (!project) return <Spin />

    return (
        <>
            <h1>{project.name}</h1>
            <p>{project.description}</p>
            <TaskList 
                id={project.id} 
                params={{
                    projectId: project.id
                }}
            />
        </>
    );
}