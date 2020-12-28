import React, { Component } from 'react';
import { Tabs } from 'antd';
import RegistrationForm from '../components/RegistrationForm';
import SignInForm from '../components/SignInForm';
import { connect } from 'react-redux';
import {Link} from "react-router-dom";

class Home extends Component {
  static displayName = Home.name;

  render () {
    console.log('state: ', this.props);
    return (
      <div>
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
        
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    user: state.user.user
  }
}

export default connect(mapStateToProps)(Home)
