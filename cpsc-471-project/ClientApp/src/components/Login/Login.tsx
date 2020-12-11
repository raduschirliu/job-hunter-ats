import { Button, TextField } from '@material-ui/core';
import React, { useContext, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useHistory } from 'react-router';
import AuthContext from '../../contexts/AuthContext';
import './Login.css';

/**
 * Login component responsible for taking in username/password inputs and sending the login request
 */
const Login = () => {
  const history = useHistory();
  const { register, handleSubmit } = useForm();
  const { login, isLoggedIn } = useContext(AuthContext);

  useEffect(() => {
    if (isLoggedIn()) {
      history.push('/companies');
    }
  }, [isLoggedIn, history]);

  const onSubmit = (data: any) => {
    login(data.username, data.password).then(res => {
    }).catch(err => {
      console.log(err);
    });
  };

  return (
    <div className="login-container">
      <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
        <TextField name="username" label="Username" inputRef={register} />
        <TextField name="password" label="Password" type="password" inputRef={register} />
        <Button variant="outlined" color="primary" type="submit">Login</Button>
      </form>
    </div>
  );
};

export default Login;
