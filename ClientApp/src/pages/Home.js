import React, { Component } from 'react';
import { Tabs } from 'antd';
import RegistrationForm from '../components/RegistrationForm';
import SignInForm from '../components/SignInForm';
import { connect } from 'react-redux';
import {Link} from "react-router-dom";
import {Projects} from "./Projects";

class Home extends Component {
  static displayName = Home.name;

  render () {
    console.log('state: ', this.props);
    return (
      <div>
        {this.props.authenticated ? <Projects /> :
          <Tabs defaultActiveKey="1">
            <Tabs.TabPane tab="Sign In" key="1">
              {
                this.props.user && <Link to = {`/user/${this.props.user.id}`}>{this.props.user.fullName}</Link>
              }
              <SignInForm />
            </Tabs.TabPane>
            <Tabs.TabPane tab="Registration" key="2">
              <RegistrationForm />  
            </Tabs.TabPane>
          </Tabs>
        }
        
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    user: state.user.user,
    token: state.user.token,
    authenticated: state.user.token !== null,
  }
}

export default connect(mapStateToProps)(Home)
