import React from 'react'
import {connect} from 'react-redux'
import {Profile} from './Profile'
import {setProfileEnrolledProjects, setProfileCreatedProjects, setProfileUser} from '../../store/profile/actions'
import {setUser, setEnrolledProjects, setCreatedProjects} from "../../store/user/actions";


const ProfileContainer = (props) => {
    return (
        <Profile {...props} />
    )
}

const mapStateToProps = (state) =>{
    return {
        user: state.user.user,
        token: state.user.token,
        authenticated: state.user.tokenChecked && state.user.token !== null,
        tokenChecked: state.user.tokenChecked,
        userProjects: state.user.createdProjects,
        userEnrolledProjects: state.user.enrolledProjects,
        
        // profileUser: state.profile.user,
        // profileCreatedProjects: state.profile.createdProjects,
        // profileEnrolledProjects: state.profile.enrolledProjects
    }
}

const mapDispatchToActions = {
    setUserProjects: setCreatedProjects,
    setUserEnrolledProjects: setEnrolledProjects,
    // setProfileCreatedProjects: setProfileCreatedProjects,
    // setProfileEnrolledProjects: setProfileEnrolledProjects,
    setUser: setUser,
    setProfileUser: setProfileUser
}

export default connect(mapStateToProps, mapDispatchToActions)(ProfileContainer)