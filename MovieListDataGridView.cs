using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MCM2MyFilms
{
    public class MovieListDataGridView : DataGridView
    {
        protected override void SetSelectedRowCore(int rowIndex, bool selected)
        {
            if (this.SelectedRows.Count > 0)
            {
                try
                {
                    AMCMovie.MovieStatus selectionStatus = ((AMCMovie)SelectedRows[0].DataBoundItem).Status;
                    if (isValidMovieStatus(selectionStatus, ((AMCMovie)Rows[rowIndex].DataBoundItem).Status))
                        base.SetSelectedRowCore(rowIndex, selected);
                }
                catch (IndexOutOfRangeException) // Control Throws this if the bound datasource is destroyed before the grid
                {
                    base.SetSelectedRowCore(rowIndex, selected);
                }
            }
            else
                base.SetSelectedRowCore(rowIndex, selected);
        }

        protected bool isValidMovieStatus(AMCMovie.MovieStatus selectionStatus, AMCMovie.MovieStatus status)
        {
            bool ret = false;
            switch (selectionStatus)
            {
                case AMCMovie.MovieStatus.Unknown:
                    break;
                case AMCMovie.MovieStatus.NoMovieFile:
                    if (status == AMCMovie.MovieStatus.NoMovieFile)
                        ret = true;
                    break;
                case AMCMovie.MovieStatus.NotInCatalog:
                    if (status == AMCMovie.MovieStatus.NotInCatalog)
                        ret = true;
                    break;
                case AMCMovie.MovieStatus.StaleData:
                case AMCMovie.MovieStatus.OK:
                    if (status == AMCMovie.MovieStatus.StaleData || status == AMCMovie.MovieStatus.OK)
                        ret = true;
                    break;
                default:
                    break;
            }
            return ret;
        }
    }
}
