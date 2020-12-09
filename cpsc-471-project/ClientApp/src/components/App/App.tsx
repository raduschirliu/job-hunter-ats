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
import Logout from '../Logout/Logout';
import Nav from '../Nav/Nav';
import Register from '../Register/Register';
import Resume from '../Resume/Resume';
import Resumes from '../Resumes/Resumes';
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
  const { isLoggedIn } = useContext(AuthContext);

  return isLoggedIn() ? <Route path={path} component={component} exact={exact} /> : <Redirect to="/login"/>;
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
            <Route path="/register" component={Register} exact />
            <Route path="/logout" component={Logout} exact />
            <ProtectedRoute path="/companies" component={Companies} exact />
            <ProtectedRoute path="/companies/:companyId" component={Company} exact />
            <ProtectedRoute path="/users" component={Users} exact />
            <ProtectedRoute path="/users/:userId" component={User} exact />
            <ProtectedRoute path="/jobposts" component={JobPosts} exact />
            <ProtectedRoute path="/jobposts/:jobPostId" component={JobPost} exact />
            <ProtectedRoute path="/resumes" component={Resumes} exact />
            <ProtectedRoute path="/resumes/:resumeId" component={Resume} exact />
            <Route path="/">
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
