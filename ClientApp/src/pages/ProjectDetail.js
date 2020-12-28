import { Spin } from 'antd';
import React, { useEffect, useState } from 'react';
import TaskList from '../components/smart/task/TaskList';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import {connect} from 'react-redux'

export const ProjectDetail = (props) => {

    const {token} = props
    
    const [project, setProject] = useState(null);

    useEffect(() => {
        if (token)
            HttpProvider.auth(router.project.one(props.match.params.id), token).then(res => {
                console.log('PROJECT',res)
                setProject(res);
            });
        else 
            HttpProvider.get(router.project.one(props.match.params.id)).then(setProject)
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

const mapStateToProps = (state) => {
    return {
        token: state.user.token,
        user: state.user.user,
        authenticated: state.user.token !== null
    }
}

export default connect(mapStateToProps)(ProjectDetail)
