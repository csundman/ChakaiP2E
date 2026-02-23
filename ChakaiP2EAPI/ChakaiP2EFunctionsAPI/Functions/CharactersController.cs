using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ChakaiP2EFunctionsAPI.Utilities;
using System.Net;
using System.Threading.Tasks;
using ChakaiP2EFunctionsAPI.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Linq;

namespace ChakaiP2EFunctionsAPI.Functions
{
    public class CharactersController : BaseController
    {
        public CharactersController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            InitializeFunction<CharactersController>(loggerFactory);
        }

        [Function("Characters")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = "characters/{id?}")] HttpRequestData req, ILogger log, string id)
        {
            InitResponse(req);

            await HandleRequest(req, id);

            return Response;
        }

        #region POST

        protected override async Task HandlePost(HttpRequestData req)
        {
            Character characterData = null;
            try
            {
                characterData = await req.ReadFromJsonAsync<Character>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            characterData.AccountId = AccountID.Value;

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                object result;

                try
                {
                    result = await SqlHelper.ExecuteInsert(conn, characterData);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }

                //If we got here, we created the character
                Response.StatusCode = HttpStatusCode.Created;
                await Response.WriteAsJsonAsync(new Dictionary<string, object>
                {
                    { "message", "Character Creation Success!" }, { "result", result }
                });
                return;
            }
        }

        #endregion

        #region MULTIGET

        protected override async Task HandleMultiGet()
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                await conn.OpenAsync();

                try
                {
                    var characters = await SqlHelper.ExecuteSelect<Character>(conn, null, AccountID);    

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(characters);
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion

        #region DELETE

        protected override async Task HandleDelete(int id)
        {
            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    var found = SqlHelper.ExecuteDelete(conn, "characters" , new Character { CharacterId = id, AccountId = AccountID.Value }, AccountID);

                    if (!found)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No character found with Id " +  id);
                    }
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion

        #region PUT

        protected override async Task HandlePut(HttpRequestData req, int id)
        {
            Character characterData = null;
            try
            {
                characterData = await req.ReadFromJsonAsync<Character>();
            }
            catch (Exception ex)
            {
                await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                return;
            }

            if (characterData == null)
            {
                await WriteErrorToResponse(HttpStatusCode.BadRequest, "Could not parse JSON");
            }

            characterData.AccountId = AccountID.Value;

            //Start the SQL connection
            using (SqlConnection conn = new SqlConnection(SecurityUtil.ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    if(characterData.PartyId == null && characterData.PartyInviteCode != null)
                    {
                        // Check if the invite code is valid
                        var party = (await SqlHelper.ExecuteSelect<Party>(conn, new Party { PartyInviteCode = characterData.PartyInviteCode })).FirstOrDefault();
                        if (party != null)
                        {
                            characterData.PartyId = party.PartyId;
                        }
                        else
                        {
                            await WriteErrorToResponse(HttpStatusCode.BadRequest, "Invalid Party Invite Code");
                            return;
                        }
                    }

                    var updatedCharacter = await SqlHelper.ExecuteUpdate(conn, characterData, id, AccountID);

                    if (updatedCharacter == null)
                    {
                        await WriteErrorToResponse(HttpStatusCode.NotFound, "No character found with Id " + id);
                    }

                    if (characterData.SpellCollections != null)
                    {
                        var wrapperTransaction = conn.BeginTransaction();
                        try
                        {
                            foreach (var spellCollection in characterData.SpellCollections.Where(sc => sc.CharacterId == 0))
                            {
                                SqlHelper.ExecuteDelete(conn, "spell_collections", new SpellCollection { SpellCollectionId = id }, null, wrapperTransaction);
                                updatedCharacter.SpellCollections.RemoveAll(sc => sc.SpellCollectionId == spellCollection.SpellCollectionId);
                            }

                            foreach (var spellCollection in characterData.SpellCollections.Where(sc => sc.SpellCollectionId != 0))
                            {
                                var updatedSpellCollection = await SqlHelper.ExecuteUpdate(conn, spellCollection, spellCollection.SpellCollectionId, null, wrapperTransaction);
                                updatedCharacter.SpellCollections.RemoveAll(sc => sc.SpellCollectionId == spellCollection.SpellCollectionId);
                                updatedCharacter.SpellCollections.Add(updatedSpellCollection);
                            }

                            foreach (var spellCollection in characterData.SpellCollections.ToList().Where(sc => sc.SpellCollectionId == 0))
                            {
                                spellCollection.CharacterId = id;
                                await SqlHelper.ExecuteInsert(conn, spellCollection, wrapperTransaction);
                                updatedCharacter.SpellCollections.Add(spellCollection);
                            }

                            wrapperTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            wrapperTransaction.Rollback();
                            await WriteErrorToResponse(HttpStatusCode.InternalServerError, "Error updating spell collections: " + ex.Message);
                            return;
                        }
                    }

                    Response.StatusCode = HttpStatusCode.OK;
                    await Response.WriteAsJsonAsync(new Dictionary<string, object>
                    {
                        { "message", "Character Update Success!" }, { "result", updatedCharacter }
                    });
                    return;
                }
                catch (Exception ex)
                {
                    await WriteErrorToResponse(HttpStatusCode.InternalServerError, ex.Message);
                    return;
                }
            }
        }

        #endregion
    }
}
