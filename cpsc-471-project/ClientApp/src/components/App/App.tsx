import React, { useContext } from 'react';
import {
  BrowserRouter as Router,
  Redirect,
  Route,
  Switch,
} from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import Companies from '../Companies/Companies';
import Company from '../Company/Company';
import JobPost from '../JobPost/JobPost';
import JobPosts from '../JobPosts/JobPosts';
import Login from '../Login/Login';
import Nav from '../Nav/Nav';
import User from '../User/User';
import Users from '../Users/Users';
import './App.css';

const ProtectedRoute = ({
  path,
  component,
  exact = false,
}: {
  path: string;
  component: any;
  exact?: boolean;
}) => {
  return <Route path={path} component={component} exact={exact} />;
};

const App = () => {
  const { isLoggedIn } = useContext(AuthContext);

  return (
    <Router>
      <div className="app-container">
        <Nav />
        <div className="app-content">
          <Switch>
            <Route path="/login" component={Login} exact />
            <Route path="/companies" component={Companies} exact />
            <Route path="/companies/:companyId" component={Company} exact />
            <Route path="/users" component={Users} exact />
            <Route path="/users/:userId" component={User} exact />
            <Route path="/jobposts" component={JobPosts} exact />
            <Route path="/jobposts/:jobPostId" component={JobPost} exact />
            <Route path="/" exact>
              {isLoggedIn() ? (
                <Redirect to="/companies" />
              ) : (
                <Redirect to="/login" />
              )}
            </Route>
          </Switch>
        </div>
      </div>
    </Router>
  );
};

export default App;
