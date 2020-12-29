import React, { useEffect, useState } from 'react';
import ProjectCard from '../components/dumb/project/ProjectCard';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import {connect}  from 'react-redux'

export const Projects = (props) => {

    const {token, user} = props
    const [projects, setProjects] = useState([]);

    
    useEffect(() => {
        HttpProvider.auth(router.project.list(), token).then(res => {
            setProjects(res)
        });
    }, []);

    
    return (
        <>
            <h1>Projects </h1>
            {projects.map((item, index) => <ProjectCard {...item} key={index}/>)}
        </>
    );
}

const mapStateToProps = (state) => {
    debugger
    return {
        token: state.user.token,
        user: state.user.user,
        authenticated: state.user.token !== null
    }
}

export default connect(mapStateToProps, {})(Projects)
