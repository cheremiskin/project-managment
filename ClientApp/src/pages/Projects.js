import React, { useEffect, useState } from 'react';
import ProjectCard from '../components/dumb/project/ProjectCard';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import {connect}  from 'react-redux'
import {Spin} from 'antd'

export const Projects = (props) => {

    const {token, user, tokenChecked, authenticated} = props
    const [projects, setProjects] = useState(null);

    
    useEffect(() => {
        if (!tokenChecked)
            return
        
        debugger
        
        if (authenticated){
            HttpProvider.auth(router.project.list(), token).then(res => {
                setProjects(res)
            });
        } else {
            HttpProvider.get(router.project.list()).then((response) => {
                setProjects(response)
            })
        }
    }, [tokenChecked]);
    
    if (!projects || !tokenChecked) 
        return <Spin />

    
    return (
        <>
            <h1>Projects </h1>
            {projects.map((item, index) => <ProjectCard {...item} key={index}/>)}
        </>
    );
}

const mapStateToProps = (state) => {
    return {
        token: state.user.token,
        user: state.user.user,
        authenticated: state.user.token !== null && state.user.tokenChecked,
        tokenChecked: state.user.tokenChecked
    }
}

export default connect(mapStateToProps, {})(Projects)
