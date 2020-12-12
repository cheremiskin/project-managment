import React, { useEffect, useState } from 'react';
import ProjectCard from '../components/dumb/project/ProjectCard';
import HttpProvider from '../HttpProvider';
import { router } from '../router';

export const Projects = (props) => {

    const [projects, setProjects] = useState([]);

    useEffect(() => {
        HttpProvider.get(router.project.list()).then(res => {
            setProjects(res.map((item, index) => <ProjectCard {...item} key={index}/>));
        });
    }, []);

    
    return (
        <>
            <h1>Projects </h1>
            {projects}
        </>
    );
}