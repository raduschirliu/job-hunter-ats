import { Button, TextField } from '@material-ui/core';
import React, { useContext, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import AuthContext from '../../contexts/AuthContext';
import './Login.css';

const Login = () => {
  const { register, handleSubmit } = useForm();
  const { login, getHeaders } = useContext(AuthContext);

  useEffect(() => {
    console.log(getHeaders());
  }, [getHeaders]);

  const onSubmit = (data: any) => {
    console.log(data);
    login(data.username, data.password).then(res => {
      console.log(res);
    }).catch(err => {

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
