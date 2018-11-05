import React from 'react';
import * as eventbriteService from '../../../services/eventbriteServices';
import LoadingSpinner from '../../layout/LoadingSpinner';
import { connect } from 'react-redux';
import { addEventBriteClickHistory } from '../../../actions/eventBriteClickHistory';

class EventBriteList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            internalEventsArr: [],
            internalEventsArrFilter: [],
            loading: true,
            events: [],
            result: [],

        };
        this.onCardDetails = this.onCardDetails.bind(this)
        this.filterList = this.filterList.bind(this);
    }

    componentDidMount() {
        eventbriteService.getAllInternalEvents()
        .then(response => {
            this.setState({
                internalEventsArr: response,
                internalEventsArrFilter: response
            })
        })
        .catch(console.error) 
        eventbriteService.getAll()
            .then(response => {
                this.setState({
                    result: response,
                    events: response
                })
            })
            .catch(console.error)
    }

    onKeyPress(event) {
        if (event.target.type != 'textarea' && event.which === 13) {
            event.preventDefault();
        }
    }
                {/* ...removed for brevity */}

    render() {                {/* ...removed for brevity */}

        const internalEvents = this.state.internalEventsArr.map(event => {
            return (
                <div class="col-xl-4 col-lg-3 col-sm-4 col-6" onClick={e => { this.onCardDetails(event.id, e) }}>

                    <div class="groups__item">
                        <a href={event.url}>
                            <div target='_blank' href={event.url} class="groups__img" style={{ marginTop: '200px' }}>
                                <img className='card-img-overlay' src={event.logo ? event.logo.url : 'http://www.otczenacts.com/images/no-image.jpg'} href={event.url} style={{ height: '200px' }}></img>
                                </div>
                                <div class="groups__info">
                                    <a href={event.url} target='_blank'><strong>{event.name.text}</strong></a><br></br>
                                    <a href={event.url} target='_blank' className='card-link'>Click here for more information</a>
                                </div>
                            </a>
                            <div class="actions">
                                <div class="dropdown actions__item">
                                    <i class="zmdi zmdi-more-vert" data-toggle="dropdown"></i>

                                    <div class="dropdown-menu dropdown-menu-right">
                                        <a class="dropdown-item" href="">Edit</a>
                                        <a class="dropdown-item" href="" data-demo-action="delete-listing">Delete</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
              
            );
        })

        return (

            <div>
                <header className="content__title">
                    <h1>EVENTBRITE LIST</h1>
                </header>
                <form class="search">
                    <div class="search__inner"><input class="search__text" type="text" onKeyPress={this.onKeyPress}
                        placeholder="Search for events" onChange={this.filterList}  />
                        <i class="zmdi zmdi-search search__helper" data-sa-action="search-close"></i></div>
                </form>
                <div className='row groups'>
                {/* ...removed for brevity */}
                    {internalEvents}
                </div>
            </div>
        )
    }
}

const mapDispatchToProps = dispatch => ({
    addEventBriteHistory: eventBriteItem => dispatch(addEventBriteClickHistory(eventBriteItem))
})

export default connect(null, mapDispatchToProps)(EventBriteList);
