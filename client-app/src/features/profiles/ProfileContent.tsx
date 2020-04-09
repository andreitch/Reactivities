import React from 'react'
import { Tab } from 'semantic-ui-react';
import ProfilePhotos from './ProfilePhotos';
import ProfileDesctiption from './ProfileDesctiption';
import ProfileFollowings from './ProfileFollowings';
import ProfileActivities from './ProfileActivities';

interface IProps {
  setActiveTab: (activeIndex: any) => void;
}

const panes = [
  {menuItem: 'About', render: () => <ProfileDesctiption />},
  {menuItem: 'Photos', render: () => <ProfilePhotos />},
  {menuItem: 'Activities', render: () => <ProfileActivities/>},
  {menuItem: 'Followers', render: () => <ProfileFollowings/>},
  {menuItem: 'Following', render: () => <ProfileFollowings/>}
]

const ProfileContent: React.FC<IProps> = ({setActiveTab}) => {
  return (
    <Tab
      menu={{fluid: true, vertical: true}}
      menuPosition='right'
      panes={panes}
      onTabChange={(e, data) => setActiveTab(data.activeIndex)}
    />
  )
}

export default ProfileContent;