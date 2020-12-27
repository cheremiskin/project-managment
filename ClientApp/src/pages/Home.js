import React, { Component } from 'react';
import { Tabs } from 'antd';
import RegistrationForm from '../components/RegistrationForm';
import SignInForm from '../components/SignInForm';
import { connect } from 'react-redux';

class Home extends Component {
  static displayName = Home.name;

  render () {
    console.log('state: ', this.props);
    return (
      <div>
        <Tabs defaultActiveKey="1">
          <Tabs.TabPane tab="Sign In" key="1">
            {this.props.state.profile.token}
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
    state: state
  }
}

export default connect(mapStateToProps)(Home)
