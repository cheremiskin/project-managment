import React from 'react';
import { Button } from 'antd';
import {connect} from 'react-redux';
import {setToken, setUser} from '../../store/user/actions';

const LogOut = (props) => {

    const logOut = () => {
        if (localStorage) {
            localStorage.removeItem('token');
            props.setToken(null);
            props.setUser(null); 
            window.location.assign('/')
        }    
    }
    
    return (
        <Button onClick={logOut}>Log Out</Button>
    )
}

export default connect(()=>({}), {setToken, setUser})(LogOut);