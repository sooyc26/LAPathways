import React from 'react'
import * as recommendationService from '../../../services/recommendationsService'
import swal from 'sweetalert2'

export default class RecommendationList extends React.Component {

    constructor(props) {
        super(props)

        this.state = {
            recommendationList: [],
            updateId:''
        }

        this.readAll = this.readAll.bind(this);
        this.onDelete = this.onDelete.bind(this);

        this.updateRecommendation = this.updateRecommendation.bind(this)
        this.createRecommendation = this.createRecommendation.bind(this)
    }

    componentDidMount() {
        this.readAll();
    }

    readAll() {
        recommendationService.readAllRecommendations()
            .then(response => {
                this.setState({
                    recommendationList: response.items
                })
            })
            .catch(console.error);
    }

    updateRecommendation(e, id) {
        this.props.history.push('/data-management/recommendation-logic/'+id)
    }

    createRecommendation() {
        this.props.history.push("/data-management/recommendation-logic/");
    }

    onDelete(e, id) {
        swal({
            title: "Are you sure you want to delete this recommendation logic?",
            text: "You won't be able to recover this!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "red",
            cancelButtonColor: '#7ac7f6',
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, keep it!",
            background: '#0f2940'
          }).then((result) => {
            if (result.value) {
                recommendationService.deleteRecommendationById(id)
                    .then(() => this.readAll());
                    swal({
                        title: "Deleted!",
                        text: "Recommendation logic has been deleted.",
                        type: "success",
                        background: '#0f2940',
                        confirmButtonColor: '#7ac7f6'
                      })
            }
        })
    }

    render() {
        const listRecommendations = this.state.recommendationList.map(recomm => {
            return (
                <tr key={recomm.id} >
                    <td>{recomm.id}</td>
                    <td>{recomm.coachExpertiseTitle}</td>
                    <td>{recomm.resourceCategoryTitle}</td>
                    <td>{recomm.description}</td>
                    <td>{recomm.whereString}</td>
                    <span>
                        <button id={recomm.id} onClick={e => this.updateRecommendation(e, recomm.id)} className="btn btn-light btn--icon" toggle="tooltip" placement="right" tooltip="edit" aria-describedby="tooltip"><i className="zmdi zmdi-edit zmdi-hc-fw"></i></button>
                        <button id={recomm.id} onClick={e => this.onDelete(e, recomm.id)} className="btn btn-light btn--icon" toggle="tooltip" placement="right" data-original-title="delete" aria-describedby="tooltip"><i className="zmdi zmdi-delete zmdi-hc-fw"></i></button>
                    </span>
                </tr>
            )
        })
        return (
            <div>
                <header className="content__title">
                    <h2>Recommendation List</h2>
                    <div className="actions">
                        <a onClick={this.createRecommendation} className="zmdi zmdi-plus-circle-o zmdi-hc-3x"></a>
                    </div>
                </header>
                {/* <RecommendationForm id={this.state.updateId} readAll={this.readAll}></RecommendationForm> */}
                <div className="card " >

                    <div className="card-body">

                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Id</th><th>Coach Expertise</th><th>Resource Category</th><th>Description</th><th>WhereString</th><th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                                {listRecommendations}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

        )
    }
}