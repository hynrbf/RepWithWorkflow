import { ClassicEditor } from "@ckeditor/ckeditor5-editor-classic";
import {
  Bold,
  Italic,
  Strikethrough,
  Underline,
} from "@ckeditor/ckeditor5-basic-styles";
import { Paragraph } from "@ckeditor/ckeditor5-paragraph";
import { Comments } from "@ckeditor/ckeditor5-comments";
import {
  SuggestionData,
  TrackChanges,
  TrackChangesData,
} from "@ckeditor/ckeditor5-track-changes";
import { Alignment } from "@ckeditor/ckeditor5-alignment";
import { Autoformat } from "@ckeditor/ckeditor5-autoformat";
import { BlockQuote } from "@ckeditor/ckeditor5-block-quote";
import { CloudServices } from "@ckeditor/ckeditor5-cloud-services";
import CKBoxPlugin from "@ckeditor/ckeditor5-ckbox/src/ckbox";
import { Essentials } from "@ckeditor/ckeditor5-essentials";
import { FontFamily, FontSize } from "@ckeditor/ckeditor5-font";
import { Heading } from "@ckeditor/ckeditor5-heading";
import { Highlight } from "@ckeditor/ckeditor5-highlight";
import {
  Image,
  ImageCaption,
  ImageResize,
  ImageStyle,
  ImageToolbar,
  ImageUpload,
  PictureEditing,
} from "@ckeditor/ckeditor5-image";
import { Link } from "@ckeditor/ckeditor5-link";
import { List } from "@ckeditor/ckeditor5-list";
import { MediaEmbed } from "@ckeditor/ckeditor5-media-embed";
import { PasteFromOffice } from "@ckeditor/ckeditor5-paste-from-office";
import { RemoveFormat } from "@ckeditor/ckeditor5-remove-format";
import { Table, TableToolbar } from "@ckeditor/ckeditor5-table";
import { Editor, EditorConfig, Plugin } from "ckeditor5/src/core";
import { User } from "@ckeditor/ckeditor5-collaboration-core";
import { CommentThreadData } from "@ckeditor/ckeditor5-comments/src/comments/commentsrepository";
import { EditorWatchdog } from "@ckeditor/ckeditor5-watchdog";
import { CKEDITOR_LICENSE_KEY } from "@/config";

class ClassicWithTrackChangesEditor extends ClassicEditor {}

ClassicWithTrackChangesEditor.builtinPlugins = [
  Alignment,
  Autoformat,
  BlockQuote,
  Bold,
  CKBoxPlugin,
  PictureEditing,
  CloudServices,
  Essentials,
  FontFamily,
  FontSize,
  Heading,
  Highlight,
  Image,
  ImageCaption,
  ImageResize,
  ImageStyle,
  ImageToolbar,
  ImageUpload,
  Italic,
  Link,
  List,
  MediaEmbed,
  Paragraph,
  PasteFromOffice,
  RemoveFormat,
  Strikethrough,
  Table,
  TableToolbar,
  Underline,
  Comments,
  TrackChanges,
  TrackChangesData,
];

ClassicWithTrackChangesEditor.defaultConfig = {
  toolbar: [
    "heading",
    "|",
    "fontsize",
    "fontfamily",
    "|",
    "bold",
    "italic",
    "underline",
    "strikethrough",
    "removeFormat",
    "highlight",
    "|",
    "numberedList",
    "bulletedList",
    "|",
    "undo",
    "redo",
    "|",
    "trackChanges",
    "comment",
    "commentsArchive",
    "|",
    "ckbox",
    "imageUpload",
    "link",
    "blockquote",
    "insertTable",
    "mediaEmbed",
  ],
  image: {
    toolbar: [
      "imageStyle:inline",
      "imageStyle:block",
      "imageStyle:side",
      "|",
      "toggleImageCaption",
      "imageTextAlternative",
      "|",
      "comment",
    ],
  },
  table: {
    contentToolbar: ["tableColumn", "tableRow", "mergeTableCells"],
    tableToolbar: ["comment"],
  },
  mediaEmbed: {
    toolbar: ["comment"],
  },
  comments: {
    editorConfig: {
      extraPlugins: [Bold, Italic, Underline, List, Autoformat],
    },
  },
};

class ClassicWithTrackChangesIntegration extends Plugin {
  constructor(editor: Editor) {
    super(editor);
  }

  init() {
    const editor = this.editor;

    // Define custom configurations
    editor.config.define("classicWithTrackChanges", {
      currentUser: null,
      users: [] as User[],
      commentThreads: [] as CommentThreadData[],
      suggestions: [] as SuggestionData[],
    });

    const usersPlugin = editor.plugins.get("Users");
    const trackChangesPlugin = editor.plugins.get(
      "TrackChanges",
    ) as TrackChanges;
    const commentsRepositoryPlugin = editor.plugins.get("CommentsRepository");

    const currentUser = editor.config.get(
      "classicWithTrackChanges.currentUser",
    );
    const users = editor.config.get("classicWithTrackChanges.users");
    const commentThreads = editor.config.get(
      "classicWithTrackChanges.commentThreads",
    );
    const suggestions = editor.config.get(
      "classicWithTrackChanges.suggestions",
    );

    // Load the users data.
    if (Array.isArray(users) && users.length) {
      for (const user of users) {
        usersPlugin.addUser(user);
      }
    }

    // Set the current user.
    if (currentUser) {
      usersPlugin.defineMe(`${currentUser}`);
    }

    // Load the comment threads data.
    if (Array.isArray(commentThreads) && commentThreads.length) {
      for (const commentThread of commentThreads) {
        commentThread.isFromAdapter = true;
        commentsRepositoryPlugin.addCommentThread(commentThread);
      }
    }

    // Load the suggestions data.
    if (Array.isArray(suggestions) && suggestions.length) {
      for (const suggestion of suggestions) {
        trackChangesPlugin.addSuggestion(suggestion);
      }
    }
  }
}

export const initialize = async (
  element: HTMLElement,
  initialData: string = "",
  config: Partial<EditorConfig> = {},
) => {
  const watchdog = new EditorWatchdog(null);

  watchdog.setCreator(async (element, config) => {
    // do something here...
    return await ClassicWithTrackChangesEditor.create(
      element as HTMLElement,
      config,
    );
  });

  watchdog.setDestructor((editor: Editor) => editor.destroy());

  if (!element) {
    throw new Error("CKEditor: No editor element has been set.");
  }

  const editorConfig = {
    initialData,
    extraPlugins: [ClassicWithTrackChangesIntegration],
    licenseKey: CKEDITOR_LICENSE_KEY,
    ...config,
  };

  await watchdog.create(element, editorConfig);
  return Promise.resolve(watchdog.editor);
};
