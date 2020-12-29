import React, { Component } from 'react';
import { Tabs } from 'antd';
import RegistrationForm from '../components/RegistrationForm';
import SignInForm from '../components/SignInForm';
import { connect } from 'react-redux';
import {Link} from "react-router-dom";
import {useState} from 'react'

const Auth = (props) =>  {
    
    const [activeKey, setActiveKey] = useState("1")
     
    return (
      <div>
          <Tabs 
              activeKey ={activeKey} 
              onChange = {(value) => {
                  setActiveKey(value)
              }}
            >
            <Tabs.TabPane tab="Sign In" key="1">
              <SignInForm />
            </Tabs.TabPane>
            <Tabs.TabPane tab="Registration" key="2">
              <RegistrationForm onRegistrationSuccess = {() => setActiveKey("1")} />  
            </Tabs.TabPane>
          </Tabs>
      </div>
    );
}

const mapStateToProps = (state) => {
  return {
    user: state.user.user,
    token: state.user.token,
    authenticated: state.user.token !== null && state.user.tokenChecked,
  }
}

export default connect(mapStateToProps)(Auth)
