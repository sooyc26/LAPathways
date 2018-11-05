import React from 'react';
import moment from 'moment';
import queryString from 'qs';
import { withCookies } from 'react-cookie';
import { connect } from 'react-redux'
import * as userProfileInfoService from '../services/userProfileInfoService';
import * as currentUserProfileServices from '../services/currentUserProfileService';
import * as userService from '../services/userService';
import UserProfileAverageRating from './dataManagement/userProfileInfo/UserProfileAverageRating';
import UserProfileAddressTemplate from './dataManagement/userProfileInfo/UserProfileAddressTemplate';
import MilestoneTab from './dataManagement/milestone/MilestoneTab';
import BusinessVenturesTab from './dataManagement/userProfileInfo/BusinessVenturesTab';
import AddressTab from './dataManagement/userProfileInfo/AddressTab';
import ChecklistTab from './dataManagement/coaches/ChecklistTab'; 

class UserProfile extends React.Component {
    constructor(props) {
        super(props);

        const value = queryString.parse(this.props.location.search.slice(1))
        const urlQuery = value.tab

        this.state = {
            userId: ''
            , userTypeId: ''
            , bio: ''
            , imageUrl: ''
            , dob: ''
            , raceEthnicityId: ''
            , ethnicity: ''
            , levelOfEducationId: ''
            , yearsInBusiness: ''
            , id: ''
            , list: ''
            , conditionMet: null
            , businessVentureList: []
            , addressList: []
            , userType: ''
            , currUserMentor: false
            , mentorId: 0
            , currentUserId: 0
            , currUserAdministrator: false
            , viewMilestone: false
            , reRender:false
            , showRatings: urlQuery
            , classToggle: 'tab-pane active show'
            , classHide: 'tab-pane fade'

            , currUserCoach: false
            , coachId: 0
            , viewChecklist: false
        };

        this.milestoneAccessCheck = this.milestoneAccessCheck.bind(this);
        this.checklistAccessCheck = this.checklistAccessCheck.bind(this);   /////////
        this.getValueFromCondition = this.getValueFromCondition.bind(this)
        this.onAppointmentPush = this.onAppointmentPush.bind(this)  // event handler for when Appointment tab is clicked. directs user to calendar
        this.toEditProfile = this.toEditProfile.bind(this)
    }
    
    componentDidMount() {
        let currentUserId = this.props.currentUserProfile.userTypeId
        let mentorId = this.props.currentUserProfile.userId

        if (parseInt(currentUserId) === 2) {// if curr user admin
            this.setState({
                currUserAdministrator: true,
                mentorId: mentorId,
                currentUserId: currentUserId
            })
        }

        if (parseInt(currentUserId) === 5) {// if curr user is mentor
            this.setState({
                currUserMentor: true,
                mentorId: mentorId,
                currentUserId: currentUserId
            })
        }

        if(parseInt(currentUserId) === 3) {// if curr user is coach
            this.setState({
                currUserCoach: true, 
                coachId: currentUserId, 
                currentUserId: currentUserId
            })
        }

        let getId = this.props.match.params.id
        if (this.props.match.params.id) {
            userProfileInfoService.readById(getId)
                .then(response => {
                    this.setState({
                        userId: response.items.userId
                        , userTypeId: response.items.userTypeId
                        , bio: response.items.bio
                        , imageUrl: response.items.imageUrl
                        , dob: response.items.dob
                        , raceEthnicityId: response.items.raceEthnicityId
                        // , raceEthnicity: response.items.ethnicity
                        , ethnicity: response.items.ethnicity
                        , levelOfEducationId: response.items.levelOfEducationId
                        , levelofEducation: response.items.education
                        , householdIncome: response.items.householdIncome
                        , yearsInBusiness: response.items.yearsInBusiness
                        , id: response.items.id
                        , firstName: response.items.firstName
                        , lastName: response.items.lastName
                        , email: response.items.email
                        , userType: response.items.userTypeId
                        , currentUserId: currentUserId
                        , reRender: true
                    })
                })
                .then(() => this.milestoneAccessCheck())
                .then(() => this.checklistAccessCheck()) 
                .catch(console.log);
        } else {
            currentUserProfileServices.readById(this.props.currentUserProfile.userId)
                .then((response) => {
                    this.setState({
                        userId: response.items[0].userId
                        , userTypeId: response.items[0].userTypeId
                        , bio: response.items[0].bio
                        , imageUrl: response.items[0].imageUrl
                        , dob: response.items[0].dob
                        , raceEthnicityId: response.items[0].raceEthnicityId
                        // , raceEthnicity: response.items[0].ethnicity
                        , ethnicity: response.items[0].ethnicity
                        , levelOfEducationId: response.items[0].levelOfEducationId
                        , levelofEducation: response.items[0].education
                        , householdIncome: response.items[0].householdIncome
                        , yearsInBusiness: response.items[0].yearsInBusiness
                        , id: response.items[0].id
                        , firstName: response.items[0].firstName
                        , lastName: response.items[0].lastName
                        , email: response.items[0].email
                        , userType: response.items[0].userTypeId
                        , currentUserId: currentUserId
                        , reRender: true
                    })
                })
                .then(() => this.milestoneAccessCheck())
                .then(() => this.checklistAccessCheck()) 
                .catch(console.error)
        }
    }

    getValueFromCondition(val) {
        this.setState({ conditionMet: val })
    }

    onAppointmentPush(e) {  // added in event handler for the Appointments tab. when tab is clicked, redirects user to the calendar page
        let profileId = this.state.userId
        this.props.history.push("/appointments/user/" + profileId);  // re-directs to url -> /appointments/`userId`
    }

    milestoneAccessCheck() {
        if (this.state.currUserMentor) {
            userService.usersMentors_GetByMentorId(this.state.mentorId)
                .then(response => {
                    console.log(response)
                    if (this.state.currUserMentor && response.items.includes(this.state.userId)) {
                        this.setState({ viewMilestone: true })
                    }
                })
                .catch(console.error);
        } else if (this.state.currentUserId === this.state.userId || this.state.currUserAdministrator)
            this.setState({ viewMilestone: true })
    }

    checklistAccessCheck() {
        if (this.state.currUserCoach && (this.props.currentUserProfile.userId !== this.state.userId)) {
            this.setState({
                viewChecklist: true
            })
        } else if (this.state.currentUserId === this.state.userId || this.state.currUserAdministrator)
            this.setState({ viewChecklist: true })
    }

    toEditProfile() {
        const userId = this.props.currentUserProfile.userId
        this.props.history.push(`/user/form/${userId}/edit`)
    }

    render() {
        let averageRatingTab = this.state.conditionMet ? <li className="nav-item">
            <div>{this.state.showRatings === 'ratings' ? <a className="nav-link active" data-toggle="tab" href="#ratings" role="tab" aria-expanded="true">Ratings</a> : <a className="nav-link" data-toggle="tab" href="#ratings" role="tab" aria-expanded="false">Ratings</a>}
                </div>
        </li>
            : null
        const clientId = this.props.match.params.id

        let editBtn;
        let currentUser = JSON.stringify(this.props.currentUserProfile.userId)
        if (this.props.match.params.id) {   
            if (this.props.match.params.id !== currentUser) {
                editBtn = <div></div>
            }
        } else {
            editBtn = 
            <a href="" className="actions__item zmdi zmdi-edit zmdi-hc-fw" onClick={this.toEditProfile}></a>
        }

        return (
            <React.Fragment>
                <div className="content__inner content__inner--sm">
                    <header className="content__title">
                        <h1>{this.state.firstName} {this.state.lastName}</h1>
                        <small>{this.state.email}</small>
                        <div className="actions">
                            {editBtn}
                        </div>
                    </header>

                    <div className="card profile">
                        <div className="profile__img">
                            <img src={`${this.state.imageUrl}`} />
                        </div>
                        <div className="profile__info">
                            <ul className="icon-list">
                                <li><i className="zmdi zmdi-accounts zmdi-hc-fw"></i>Race/Ethnicity: {this.state.ethnicity?this.state.ethnicity:this.state.raceEthnicityId}</li>
                                <li><i className="zmdi zmdi-cake zmdi-hc-fw"></i>Date of Birth: {moment(this.state.dob).utc().format('MM/DD/YYYY')}</li>
                                <li><i className="zmdi zmdi-calendar-alt zmdi-hc-fw"></i>Years in Business: {this.state.yearsInBusiness}</li>
                                <li><i className="zmdi zmdi-trending-up zmdi-hc-fw"></i>Level of Education: {this.state.levelofEducation}</li>
                                <li><i className="zmdi zmdi-money zmdi-hc-fw"></i>Household Income: ${this.state.householdIncome}</li>
                            </ul>
                        </div>
                    </div>

                    <div className="card">
                        <div className="card-profile">
                            <div className="tab-container">
                                <ul className="nav nav-tabs" role="tablist">
                                    <li className="nav-item">
                                    <div>
                                        {this.state.showRatings === 'ratings' ? <a className="nav-link" data-toggle="tab" href="#about" role="tab" aria-expanded="false">About</a> : <a className="nav-link active" data-toggle="tab" href="#about" role="tab" aria-expanded="true">About</a>}
                                    </div>
                                        
                                    </li>
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#address" role="tab" aria-expanded="false">Contacts</a>
                                    </li>
                                    {averageRatingTab}
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#businessVenture" role="tab" aria-expanded="false">Business Ventures</a>
                                    </li>
                                    {this.state.viewMilestone || this.state.currUserAdministrator ?
                                        <li className="nav-item">
                                            <a className="nav-link" data-toggle="tab" href="#milestone" role="tab" aria-expanded="false">Milestones</a>
                                        </li>
                                        : null}
                                    <li className='nav-item'>
                                        <a className='nav-link' data-toggle='tab' href='#appointments' role='tab' aria-expanded='false' onClick={e => this.onAppointmentPush()}>Appointments</a>
                                    </li>

                                    {this.state.viewChecklist || this.state.currUserAdministrator ?
                                        <li className="nav-item">
                                            <a className="nav-link" data-toggle="tab" href="#checklist" role="tab" aria-expanded="false">Checklist</a>
                                        </li>
                                        : null}
                                    </ul>



                                <div className="tab-content">
                                    <div className={this.state.showRatings === 'ratings' ? this.state.classHide : this.state.classToggle} id="about" role="tabpanel" aria-expanded='true'>
                                        <div className="card-body">
                                            {this.state.bio}
                                        </div>
                                    </div> 
                                    <div className="tab-pane fade" id="address" role="tabpanel" aria-expanded="false">
                                        <div className="card-body">
                                            {this.state.reRender ?
                                                <AddressTab userId={this.state.userId}></AddressTab> : "loading"}
                                        </div>
                                    </div>
                                    <div className="tab-pane fade" id="businessVenture" role="tabpanel" aria-expanded="false">
                                        <div className="card-body">
                                            {this.state.reRender ?
                                                <BusinessVenturesTab userId={this.state.userId} ></BusinessVenturesTab> : "loading"}
                                        </div>
                                    </div>
                                    <div className="tab-pane fade" id="milestone" role="tabpanel" aria-expanded="false">
                                        <div className="card-body">
                                            {this.state.viewMilestone ?
                                                <MilestoneTab isMentor={this.state.currUserMentor} isAdmin={this.state.currUserAdministrator}
                                                    mentorId={this.state.mentorId} userId={this.state.userId}>
                                                </MilestoneTab>
                                                : "loading"}
                                        </div>
                                    </div>

                                    <div className="tab-pane fade" id="checklist" role="tabpanel" aria-expanded="false">
                                        <div className="card-body">
                                            {this.state.viewChecklist ?
                                                <ChecklistTab isCoach={this.state.currUserCoach} isAdmin={this.state.currUserAdministrator}
                                                    coachId={this.state.coachId} userId={this.state.userId} clientId={clientId}>
                                                </ChecklistTab>
                                                : "loading"}
                                        </div>
                                    </div>

                                    <div className={this.state.showRatings === 'ratings' ? this.state.classToggle : this.state.classHide} id="ratings" role="tabpanel" aria-expanded='true'>
                                        <UserProfileAverageRating 
                                            userId={this.props.match.params.id} 
                                            userTypeId={this.state.userTypeId} 
                                            sendData={this.getValueFromCondition} />
                                    </div> 
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </React.Fragment>
        )
    }
}

const mapStateToProps = state => ({
    currentUserProfile: state.userProfiles
})
export default withCookies(connect(mapStateToProps)(UserProfile))
