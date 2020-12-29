import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Container } from 'reactstrap';
import HttpProvider from '../HttpProvider';
import { router } from '../router';
import { setUser, setToken, setTokenChecked } from '../store/user/actions';
import NavMenu from './NavMenu';

class Layout extends Component {
  static displayName = Layout.name;

  componentDidMount() {
    if (localStorage) {
      let token = localStorage.getItem('token');

      if (token) {
        token = JSON.parse(localStorage.getItem('token'))
      }

      if (token !== null) {

        console.log("token ->", token);
        this.props.setToken(token)
        
        HttpProvider.auth(router.user.me, token)
            .then(
              (res) => {
                this.props.setUser(res);
                this.props.setTokenChecked(true)
              }
            )
      } else {
        this.props.setTokenChecked(true)
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


export default connect(() => ({}), {setUser, setToken, setTokenChecked})(Layout);