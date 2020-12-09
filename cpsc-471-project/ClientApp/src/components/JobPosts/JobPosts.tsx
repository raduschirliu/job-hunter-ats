import { CircularProgress } from '@material-ui/core';
import axios from 'axios';
import React, { useContext, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../contexts/AuthContext';
import { IJobPost } from '../../models/IJobPost';
import './JobPosts.css';

const JobPostCard = ({ jobPost }: { jobPost: IJobPost }) => {
  return (
    <Link to={`/jobposts/${jobPost.jobPostId}`}>
      <div className="jobpost-card">
        <p>{jobPost.name}</p>
      </div>
    </Link>
  );
};

const JobPosts = () => {
  const { getHeaders } = useContext(AuthContext);
  const [jobPosts, setJobPosts] = useState<IJobPost[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (loading) setLoading(true);
    axios
      .get('/api/jobposts', getHeaders())
      .then((res) => {
        console.log(res.data);
        setJobPosts(res.data);
      })
      .catch((err) => console.log(err))
      .finally(() => setLoading(false));
  }, [loading, getHeaders]);

  return (
    <div className="jobposts-container">
      {loading ? (
        <CircularProgress />
      ) : (
        <>
          {jobPosts.map((jobPost) => (
            <JobPostCard key={jobPost.jobPostId} jobPost={jobPost} />
          ))}
        </>
      )}
    </div>
  );
};

export default JobPosts;
