import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Container } from 'reactstrap';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import { setUser } from '../store/user/actions';
import { NavMenu } from './NavMenu';

class Layout extends Component {
  static displayName = Layout.name;

  componentDidMount() {
    if (localStorage) {
      let token = localStorage.getItem('token');

      if (token) {
        token = JSON.parse(localStorage.getItem('token'))
      }

      if (token !== null) {
        HttpProvider.auth(router.user.me, token).then(
          (res) => {
            this.props.setUser(res); 
          }
        )
      }
    }
  }

  render () {
    return (
      <div>
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }
}


export default connect(() => ({}), {setUser})(Layout);