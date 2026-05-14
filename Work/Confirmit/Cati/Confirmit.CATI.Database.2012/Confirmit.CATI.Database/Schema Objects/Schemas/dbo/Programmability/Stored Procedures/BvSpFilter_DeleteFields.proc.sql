/* This sp don't change survey specification of filter.
   We should execute this sp change filter fields (delete/insert)
 */
CREATE PROCEDURE [dbo].[BvSpFilter_DeleteFields]
@FilterSID   INTEGER
AS
	DELETE FROM BvFilterFields WHERE FilterSID = @FilterSID
RETURN (0)