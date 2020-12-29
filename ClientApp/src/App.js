import React, { Component } from 'react';
import ProjectDetail from './pages/ProjectDetail';
import Projects from './pages/Projects';
import './assets/styles/App.css';
import UserList from "./pages/UserList";
import Task from "./pages/Task";
import { Route } from 'react-router';
import Layout from './components/Layout';
import Profile from './pages/profile/ProfileContainer'

import {  Provider } from 'react-redux'
import { createStore } from "redux";
import rootReducer from "./store/reducers";
import { middleware } from './middleware';
import Auth from "./pages/Auth";

const store = createStore(rootReducer, {}, middleware)

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (
      <Provider store={store}>
        <Layout>
          <Route exact path='/auth' component={Auth} />
          <Route exact path='/users' component={UserList} />
          <Route path='/user/:id' component={Profile} />
          <Route path='/task/:id' component={Task} />
          <Route path='/projects/' component={Projects} />
          <Route exact path='/' component = {Projects} />
          <Route path='/project/:id' component={ProjectDetail} />
        </Layout>
      </Provider>
    );
  }
}
