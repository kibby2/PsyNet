using Microsoft.AspNetCore.Mvc;
using PsyNet.Web.Models.ViewModels;
using PsyNet.Web.Models.Domain;
using PsyNet.Web.Repositories;

namespace PsyNet.Web.Controllers
{
    public class AdminTagsController : Controller

    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult AddTag()
        {
            return View();
        }
        [HttpPost]
        [ActionName("AddTag")]

        public async Task<IActionResult> AddTag(AddTagRequest addTagRequest)
        {
            //Mapping AddTagRequest to Tag Domain model
            var tag = new Tag
            {
                Name = addTagRequest.TagName,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("TagList");
        }
        [HttpGet]

        public async Task<IActionResult> TagList()
        {
            var tags = await tagRepository.GetAllAsync();

            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> EditTag(Guid Id)
        {
            var tag = await tagRepository.GetAsync(Id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };

                return View(editTagRequest);
            }

            return View(null);

        }
        [HttpPost]
        public async Task<IActionResult> EditTag(EditTagRequest editTagRequest)
        {

            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                //show a success notification
                return RedirectToAction("TagList");
            }

            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTagRequest.Id);
            if (deletedTag != null)
            {
                //show a success notification
                return RedirectToAction("TagList");
            }
            //show an error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });


            //wow-fit 12345 kt aditiy
        }
    }
}
