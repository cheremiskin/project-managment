import React from 'react';
import { Button } from 'antd';
import {connect} from 'react-redux';
import {setToken, setUser} from '../../store/user/actions';
import '../../assets/styles/components/LogOut.css';

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
        <Button className="logout-button" onClick={logOut}>Log Out</Button>
    )
}

export default connect(()=>({}), {setToken, setUser})(LogOut);